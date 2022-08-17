using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    [SerializeField] private int saveFileNum;
    [SerializeField] private GameObject deleteButton;
    private string _path;

    void Start()
    {
        _path = Application.dataPath;
        _path += string.Format("/Data/Player/PlayerData{0}.json", saveFileNum);

        FileInfo fileInfo = new FileInfo(_path);

        if(!fileInfo.Exists)
        {
            deleteButton.GetComponent<Button>().enabled = false;
        }
    }

    public async void StartNewGame()
    {
        FileInfo fileInfo = new FileInfo(_path);

        long totalCopied = 0;

        if(fileInfo.Exists)
        {
            DeleteSaveFile();
        }

        GameManager.Instance.CurrentStage = 102;

        await CopyAsync(Application.dataPath + "/Data/Player/PlayerData.json", _path);
        await CopyAsync(Application.dataPath + "/Data/Card/PlayerCard.json", 
            Application.dataPath + string.Format("/Data/Card/PlayerCard{0}.json", saveFileNum));
        //await CardData.Instance._loadnew(string.Format("PlayerCard{0}", saveFileNum));

        GameManager.Instance.PlayerNum = saveFileNum;

        Debug.Log(totalCopied);

        LoadingManager.Instance.LoadWorldMap(true);
    }


    static async Task<long> CopyAsync(string FromPath, string ToPath)
    {
        using (var fromStream = new FileStream(FromPath, FileMode.Open))
        {
            long totalCopied = 0;

            using (var toStream = new FileStream(ToPath, FileMode.Create))
            {
                byte[] buffer = new byte[4096];
                int nRead = 0;
                while((nRead = await fromStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await toStream.WriteAsync(buffer, 0, nRead);
                    totalCopied += nRead;
                }
            }

            return totalCopied;
        }
    }

    public void LoadGame()
    {
        FileInfo fileInfo = new FileInfo(_path);

        if(fileInfo.Exists)
        {
            GameManager.Instance.CurrentStage = PlayerData.Instance._load(string.Format("PlayerData{0}", saveFileNum)).CurrentStage;
        }

        GameManager.Instance.PlayerNum = saveFileNum;

        LoadingManager.Instance.LoadWorldMap(true);
    }

    public void DeleteSaveFile()
    {
        FileInfo fileInfo = new FileInfo(_path);

        if(fileInfo.Exists)
        {
            File.Delete(_path);
        }
    }
}
