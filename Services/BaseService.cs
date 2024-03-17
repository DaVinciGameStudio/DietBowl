using DietBowl.EF;
using Microsoft.EntityFrameworkCore;


namespace DietBowl.Services
{
    public abstract class BaseService
    {
        protected readonly DietBowlDbContext _dietBowlDbContext;
        //protected readonly DietBowlDbContext _mapper;

        //public BaseService(MyDbContext dbContext, IMapper mapper)
        //{
        //    _dbContext = dbContext;
        //    _mapper = mapper;
        //}

        public BaseService(DietBowlDbContext dbContext)
        {
            _dietBowlDbContext = dbContext;
        }
    }
}
