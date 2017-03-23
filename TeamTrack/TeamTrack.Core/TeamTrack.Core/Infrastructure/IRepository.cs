using TeamTrack.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace TeamTrack.Core.Infrastructure
{
    public interface IRepository
    {
        #region Project

        IQueryable<Project> GetProjects(bool includeCards = false);
        Project FindProject(int id, bool includeCards = false);
        bool AddProject(Project project);
        bool UpdateProject(Project project);

        #endregion

        #region Card

        IQueryable<Card> GetCards(bool includeUserCards = false);
        Card FindCard(int id, bool includeUserCards = false);
        bool AddCard(Card card);
        bool UpdateCard(Card card);

        #endregion  

        #region Common

        bool Save();
        Task<bool> SaveAsync();

        #endregion

    }
}
