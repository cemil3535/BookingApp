using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Data.Entities
{
    public class FeatureEntity : BaseEntity
    {
        public string Title { get; set; }



        // Relational Property veri tablosuna donusmeyecek bunun icin yorum satiri olarak Relational Property yazdik diger yapilardan ayirmak icin

        public ICollection<HotelFeatureEntity> HotelFeatures { get; set; }
    }

    public class FeatureCongifuration : BaseConfiguration<FeatureEntity>
    {
        public override void Configure(EntityTypeBuilder<FeatureEntity> builder)
        {
            base.Configure(builder); 
            // FeatureEntity ilave birseyler yazacaksak buraya ekleyebiliriz.
        }
    }
}
