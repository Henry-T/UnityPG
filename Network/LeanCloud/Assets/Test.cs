using UnityEngine;
using System.Collections;
using AVOSCloud;
using System.Collections.Generic;

public class Test : MonoBehaviour
{

    private AVUser user;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (GUILayout.Button("创建一个士兵"))
        {
            AVUser.LogInAsync("games", "123").ContinueWith(loginTask=>{
                Debug.Log("login success!");
                user = (AVUser)loginTask.Result;

                AVUser otherUser = null;
                AVUser.Query.GetAsync("55220263e4b07952a95fbf7a").ContinueWith(t=>{
                    otherUser = (AVUser)t.Result;

                    AVObject soldier = new AVObject("Soldier"){
                        {"type", 3},
                        {"lv", 1},
                        {"exp", 50},
                        {"user2", new List<AVUser>(){
                            user,
                            otherUser
                        }}
                    };

                    soldier.SaveAsync().ContinueWith(saveTask =>
                        {
                            Debug.Log("solider created!");
                        });
                    });
            });
        }
    }
}
