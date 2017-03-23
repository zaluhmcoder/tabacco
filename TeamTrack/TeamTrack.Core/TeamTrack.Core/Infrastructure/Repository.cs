using System;
using System.Linq;
using System.Threading.Tasks;
using TeamTrack.Core.Data;
using TeamTrack.Core.Entities;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace TeamTrack.Core.Infrastructure
{
    public class Repository : IRepository, IDisposable
    {
        #region Constructors

        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly TeamTrackDbContext _context;
        public Repository(TeamTrackDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Project

        public IQueryable<Project> GetProjects(bool includeCards = false)
        {
            var query = _context.Projects.AsQueryable();
            if (includeCards)
            {
                    query = query.Include(x=>x.Cards);
            }
            return query;
        }
        public Project FindProject(int id, bool includeCards = false)
        {
            var query = _context.Projects.AsQueryable();
            if (includeCards)
            {
                query = query.Include(x => x.Cards);
            }
            return query.FirstOrDefault(x => x.Id == id);
        }
        public bool AddProject(Project project)
        {
            try
            {
                _context.Projects.Add(project);
                return true;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }
        public bool UpdateProject(Project project)
        {
            try
            {
                _context.Entry(project).State = EntityState.Modified;
                return true;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }

        #endregion
        
        #region Card

        public IQueryable<Card> GetCards(bool includeUserCards = false)
        {
            var query = _context.Cards.AsQueryable();
            if (includeUserCards)
            {
                query = query.Include(x => x.UserCards);
            }
            return query;
        }
        public Card FindCard(int id, bool includeUserCards = false)
        {
            var query = _context.Cards.AsQueryable();
            if (includeUserCards)
            {
                query = query.Include(x => x.UserCards);
            }
            return query.FirstOrDefault(x => x.Id == id);
        }
        public bool AddCard(Card card)
        {
            try
            {
                _context.Cards.Add(card);
                return true;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }
        public bool UpdateCard(Card project)
        {
            try
            {
                _context.Entry(project).State = EntityState.Modified;
                return true;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }

        #endregion  

        #region Common

        public bool Save()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }
        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                return false;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion
    }
}
