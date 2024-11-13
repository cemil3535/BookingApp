using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Data.UnitOfWork
{
    public  interface IUnitOfWork : IDisposable
    {

        Task<int> SaveChangesAsync();
        // Kac kayda etki ettigini geriye doner, o yuzden int.


        Task BeginTransaction();
        // Task -> Asenkron metodlarin voidi gibi dusunulebilir.


        Task CommitTransaction();


        Task RollBacTransaction();


    }
}
