using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UserManagerTests
{
    [TestClass]
    public class UserManagerTests
    {
        [TestMethod]
        public void GetAndUpdateList_IDOfFirstAndFourthUsersShouldBeTheSameInBothListAndInsertedArray()
        {
            UserManager.UserManager users = new UserManager.UserManager();
            List<User> userList = new List<User>();
            userList.AddRange(new User[] { new User("1"), new User("2"), new User("3"), new User("4")});
            users.UpdateUserList(userList);
            Assert.AreEqual("1", users.GetListOfUsers()[0].ID);
            Assert.AreEqual("4", users.GetListOfUsers()[3].ID);
        }
        [TestMethod]
        public void GetUserListOfSongs_IDOfSecondSongShouldBe10()
        {
            UserManager.UserManager users = new UserManager.UserManager();
            List<User> userList = new List<User>();
            List<Song> userSongList1 = new List<Song>(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4)});
            List<Song> userSongList2 = new List<Song>(new Song[] { new Song(5), new Song(6), new Song(7), new Song(8) });
            List<Song> userSongList3 = new List<Song>(new Song[] { new Song(9), new Song(10), new Song(11), new Song(12) });
            List<Song> userSongList4 = new List<Song>(new Song[] { new Song(13), new Song(14), new Song(15), new Song(16) });
            userList.AddRange(new User[] { new User("1", "1", userSongList1), new User("2", "2", userSongList2), 
                                new User("3", "3", userSongList3), new User("4", "4", userSongList4) });
            users.UpdateUserList(userList);
            Assert.AreEqual(10, users.GetUserListOfSongs("3")[1].ID);
        }
        [TestMethod]
        public void AddUser_IDOfSecondSongShouldBe2()
        {
            UserManager.UserManager users = new UserManager.UserManager();
            List<Song> userSongList = new List<Song>(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            users.AddUser(new User("1", "1", userSongList));
            Assert.AreEqual(2, users.GetUserListOfSongs("1")[1].ID);
        }
        [TestMethod]
        public void UpdateUserListOfSongs_IDOfFourthSongShouldBe8()
        {
            UserManager.UserManager users = new UserManager.UserManager();
            List<Song> userSongList = new List<Song>(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            List<Song> userSongListUpdated = new List<Song>(new Song[] { new Song(5), new Song(6), new Song(7), new Song(8) });
            users.AddUser(new User("1", "1", userSongList));
            users.UpdateUserListOfSongs("1", userSongListUpdated);
            Assert.AreEqual(8, users.GetUserListOfSongs("1")[3].ID);
        }
        [TestMethod]
        public void RemoveUser_ShouldReturnStatusError()
        {
            UserManager.UserManager users = new UserManager.UserManager();
            List<User> userList = new List<User>();
            userList.AddRange(new User[] { new User("1"), new User("2"), new User("3"), new User("4")});
            users.UpdateUserList(userList);
            users.RemoveUser("2");
            Assert.AreEqual(null, users.GetUserListOfSongs("2"));
        }
    }
}
