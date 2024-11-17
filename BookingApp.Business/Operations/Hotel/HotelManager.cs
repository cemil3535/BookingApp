using BookingApp.Business.Operations.Hotel.Dtos;
using BookingApp.Business.Types;
using BookingApp.Data.Entities;
using BookingApp.Data.Repositories;
using BookingApp.Data.UnitOfWork;
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
    }
}
