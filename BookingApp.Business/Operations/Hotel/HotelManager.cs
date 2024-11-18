using BookingApp.Business.Operations.Hotel.Dtos;
using BookingApp.Business.Types;
using BookingApp.Data.Entities;
using BookingApp.Data.Repositories;
using BookingApp.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Business.Operations.Hotel
{
    public class HotelManager : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<HotelEntity> _hotelRepository;
        private readonly IRepository<HotelFeatureEntity> _hotelFeatureRepository;

        public HotelManager(IUnitOfWork unitOfWork, IRepository<HotelEntity> hotelRepository, IRepository<HotelFeatureEntity> hotelFeatureRepository)
        {
            _unitOfWork = unitOfWork;
            _hotelRepository = hotelRepository;
            _hotelFeatureRepository = hotelFeatureRepository;
        }


        public async Task<ServiceMessage> AddHotel(AddHotelDto hotel)
        {
            var hasHotel = _hotelRepository.GetAll(x => x.Name.ToLower() == hotel.Name.ToLower()).Any();

            if (hasHotel) 
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu otel zaten sistemde mevcut."
                };
            }

            await _unitOfWork.BeginTransaction();

            var hotelEntity = new HotelEntity
            {
                Name = hotel.Name,
                Stars = hotel.Stars,
                Location = hotel.Location,
                AccomodationType = hotel.AccomodationType
            };

            _hotelRepository.Add(hotelEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Otel kaydi sirasinda bir sorunla karsilasildi.");
            }

            foreach (var featureId in hotel.FeatureIds)
            {

                var hotelFeature = new HotelFeatureEntity
                {
                    HotelId = hotelEntity.Id,
                    FeatureId = featureId,
                };

                _hotelFeatureRepository.Add(hotelFeature);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

            }
            catch (Exception)
            {
                await _unitOfWork.RollBacTransaction();

                throw new Exception("Otel ozellikleri eklenirken bir hatayla krsilasildi, surec basa sarildi.");
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };

        }


        public async Task<ServiceMessage> AddJustHotelStars(int id, int changeTo)
        {
            var hotel = _hotelRepository.GetById(id);

            if (hotel is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu id ile eslesen otel bulunamadi."
                };
            }

            hotel.Stars = changeTo;

            _hotelRepository.Update(hotel);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Yildiz sayisi degistirlirken bir hata olustu.");
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }


        public async Task<ServiceMessage> DeleteHotel(int id)
        {
            var hotel = _hotelRepository.GetById(id);

            if(hotel is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silinmek istenen otel bulunamadi."
                };
            }

            _hotelRepository.Delete(id);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Hotel silme Islemi sirasinda bir hata oldu.");
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }


        public async Task<HotelDto> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetAll(x => x.Id == id)
                .Select(x => new HotelDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Stars = x.Stars,
                    Location = x.Location,
                    AccomodationType= x.AccomodationType,
                    Features = x.HotelFeatures.Select(f=> new HotelFeatureDto
                    {
                        Id =f.Id,
                        Title = f.Feature.Title
                    }).ToList()
                }).FirstOrDefaultAsync();

            return hotel;

        }


        public async Task<List<HotelDto>> GetHotels()
        {
            var hotels = await _hotelRepository.GetAll()
               .Select(x => new HotelDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   Stars = x.Stars,
                   Location = x.Location,
                   AccomodationType = x.AccomodationType,
                   Features = x.HotelFeatures.Select(f => new HotelFeatureDto
                   {
                       Id = f.Id,
                       Title = f.Feature.Title
                   }).ToList()
               }).ToListAsync();

            return hotels;
        }

        public async Task<ServiceMessage> UpdateHotel(UpdateHotelDto hotel)
        {
            var hotelEntity = _hotelRepository.GetById(hotel.Id);

            if (hotelEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Otel bulunamadi."
                };
            }

            await _unitOfWork.BeginTransaction();

            hotelEntity.Name = hotel.Name;
            hotelEntity.Stars = hotel.Stars;
            hotelEntity.Location = hotel.Location;
            hotelEntity.AccomodationType =hotel.AccomodationType;

            _hotelRepository.Update(hotelEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollBacTransaction();
                throw new Exception("Otel bilgileri guncellenirken bir hata ile karsilasildi.");
            }

            var hotelFeatures = _hotelFeatureRepository.GetAll(x => x.HotelId ==x.HotelId).ToList();
            foreach(var hotelFeature in hotelFeatures)
            {
                _hotelFeatureRepository.Delete(hotelFeature, false); // HARD DELETE 
            }

            foreach(var featureId in hotel.FeatureIds)
            {
                var hotelFeature = new HotelFeatureEntity
                {
                    HotelId = hotelEntity.Id,
                    FeatureId = featureId
                };

                _hotelFeatureRepository.Add(hotelFeature);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {

                await _unitOfWork.RollBacTransaction();
                throw new Exception("Otel bilgileri guncellenirken bir hata olustu.Islemleri geriye aliniyor.");
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }
    }
}
