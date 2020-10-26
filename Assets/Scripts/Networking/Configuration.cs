using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BuildType{

    LOCAL_SERVER,
    LOCAL_ClIENT,
    REMOTE_SERVER,
    REMOTE_CLIENT

}
public class Configuration : MonoBehaviour
{
    public BuildType gameBuildType;
    public string playfabBuildID;
    public string serverIpAdress = "";
    public ushort portNumber ;
    public bool playFabDebugging;
    
}
