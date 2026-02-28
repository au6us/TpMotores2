using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class RemoteConfig : MonoBehaviour
{
    public struct userAttributes { }
    public struct appAttributes { }

    public int playerLife;
    public string enemyName1;
    public string enemyName2;
    public int appVersion;
    public bool serverOut;
    public float playerSpeed;    
    public string welcomeMessage; 
    public float jumpForce;  
    public int startCoins;    


    async Task InitializeRemoteConfigAsync()
    {
        
        await UnityServices.InitializeAsync();

        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    async void Start()
    {
        Debug.Log("PASO 1: El script RemoteConfig arrancó. (Si no ves esto, el objeto no está en la escena).");
        // ELIMINAMOS O COMENTAMOS ESTO PARA QUE NO BLOQUEE
        // if (Utilities.CheckForInternetConnection())
        // {
        await InitializeRemoteConfigAsync();
        // }

        RemoteConfigService.Instance.FetchCompleted += OnFetchDataCompleted;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }


    void OnFetchDataCompleted(ConfigResponse response)
    {
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());

        playerLife = RemoteConfigService.Instance.appConfig.GetInt("PlayerLife");
        enemyName1 = RemoteConfigService.Instance.appConfig.GetString("EnemyName1");
        enemyName2 = RemoteConfigService.Instance.appConfig.GetString("EnemyName2");
        appVersion = RemoteConfigService.Instance.appConfig.GetInt("AppVersion");
        serverOut = RemoteConfigService.Instance.appConfig.GetBool("ServerOut");
        playerSpeed = RemoteConfigService.Instance.appConfig.GetFloat("PlayerSpeed", 5.0f);
        welcomeMessage = RemoteConfigService.Instance.appConfig.GetString("WelcomeMsg", "Hola");
        jumpForce = RemoteConfigService.Instance.appConfig.GetFloat("JumpForce", 6.0f);
        startCoins = RemoteConfigService.Instance.appConfig.GetInt("StartCoins", 0);


        //PUENTE AL PLAYER
        Player elJugador = FindObjectOfType<Player>();
        if (elJugador != null)
        {
            elJugador.speed = playerSpeed;
            elJugador.jumpForce = jumpForce;

            //PUENTE DE MONEDAS
            if (startCoins > 0)
            {
                for (int i = 0; i < startCoins; i++) elJugador.CollectCoin();
            }
        }

        // PUENTE A LOS ENEMIGOS
        Entity[] todosLosEnemigos = FindObjectsOfType<Entity>();

        foreach (Entity enemigo in todosLosEnemigos)
        {
            // Si tiene el ID "enemy_1", le ponemos el Nombre 1
            if (enemigo.enemyID == "enemy_1")
            {
                enemigo.ConfigureEntity(enemyName1, 0); //0 Sería la vida
            }
            // Si tiene el ID "enemy_2", le ponemos el Nombre 2
            else if (enemigo.enemyID == "enemy_2")
            {
                enemigo.ConfigureEntity(enemyName2, 0);
            }
        }

        Debug.Log("Listoorti, datos aplicados");
    }
}
