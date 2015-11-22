using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class InfrastructureTests
    {
        [TestMethod]
        public void SaveAndLoadListOfUsers_IDOfFirstElementsOfSavedListAndLoadedListsShouldBeTheSame()
        {
            Infrastructure.Infrastructure infrastructure = new Infrastructure.Infrastructure();
            List<Song> songList = new List<Song>();
            Song song1 = new Song(1);
            Song song2 = new Song(2);
            songList.AddRange(new Song[] { song1, song2 });
            List<User> userList = new List<User>();
            User user1 = new User("some1", "id1", songList);
            User user2 = new User("some2", "id2", songList);
            userList.AddRange(new User[] { user1, user2 });
            infrastructure.SaveListOfUsers(userList);
            List<User> userList2 = new List<User>();
            userList2 = infrastructure.LoadListOfUsers();
            Assert.AreEqual(userList[1].SongList[1].ID, userList2[1].SongList[1].ID, "Сериализация прошла некорректно");
        }
    }
}
