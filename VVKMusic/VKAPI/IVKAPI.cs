using Common;

namespace VKAPI
{
    interface IVKAPI
    {
        string Auth();
        string GetResponse(string linkToSong);
        Song[] GetAudio();
    }
}
