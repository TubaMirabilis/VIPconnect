using System.Threading.Tasks;
using Tweetinvi;

namespace ProjectX
{
    public static class Extensions
    {
        public static async Task<string> GetImageAsync(this TwitterClient client, string id)
        {
            var user = await client.UsersV2.GetUserByIdAsync(id);
            return user.User.ProfileImageUrl;
        }
    }
}