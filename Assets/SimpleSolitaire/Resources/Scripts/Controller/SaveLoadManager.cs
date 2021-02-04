using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleSolitaire.Controller
{
    public static class SaveLoadManager
    {
        public static void SaveAdsInfo(AdsManager adsManager)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/adsData.ads";
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerData data = new PlayerData(adsManager);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static PlayerData LoadAdsData ()
        {
            string path = Application.persistentDataPath + "/adsData.ads";

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return data;
            }
            else
            {
                Debug.LogError("Save file not found in " + path);
                return null;
            }
        }
    }
}
