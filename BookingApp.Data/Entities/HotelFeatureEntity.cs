using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Data.Entities
{
    public class HotelFeatureEntity : BaseEntity
    {
        // Hangi otlede hangi ozellik var?
        public int HotelId { get; set; }

        public int FeatureId {  get; set; }



        // Relational Property

        public HotelEntity Hotel { get; set; }

        public FeatureEntity Feature { get; set; }
    }

    public class HotelFeatureConfiguration : BaseConfiguration<HotelFeatureEntity>
    {
        public override void Configure(EntityTypeBuilder<HotelFeatureEntity> builder)
        {
            builder.Ignore(x => x.Id); // Id property'sini gormezden geldik, tabloya aktarilmayacak
            builder.HasKey("HotelId", "FeatureId");
            // Composite Key olusturup yeni Primary Key olarak atadik.

            base.Configure(builder);
        }
    }

}
