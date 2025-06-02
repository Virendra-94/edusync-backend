using System.Threading.Tasks;

namespace EduSync.API.Interfaces
{
    public interface IEventHubService
    {
        Task SendQuizAttemptAsync(object quizAttemptData);
    }
}