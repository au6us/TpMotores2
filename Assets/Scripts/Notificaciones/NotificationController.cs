using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Notifications.Android;

public class NotificationController : MonoBehaviour
{
    // ID del canal (debe ser único)
    private const string idCanal = "canal_froggy";

    private void Start()
    {
        // 1. Pedir permisos (Obligatorio para Android 13+)
        StartCoroutine(SolicitarPermisos());

        // 2. Crear el canal apenas arranca el juego
        CrearCanalNotificaciones();

        // 3. Borrar notificaciones viejas si el jugador ya entró
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }

    // Esta función detecta si MINIMIZAS la app
    private void OnApplicationFocus(bool focus)
    {
        if (focus == false)
        {
            // Si te fuiste (focus false) Programamos el aviso
            ProgramarNotificacion();
        }
        else
        {
            // Si volviste (focus true) Cancelamos el aviso para que no joda
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }
    }

    private void CrearCanalNotificaciones()
    {
        var canal = new AndroidNotificationChannel()
        {
            Id = idCanal,
            Name = "Entrenamiento Froggy",
            Description = "Recordatorios para volver a jugar",
            Importance = Importance.Default
        };
        AndroidNotificationCenter.RegisterNotificationChannel(canal);
    }

    public void ProgramarNotificacion()
    {
        // Configurar el mensaje
        var notificacion = new AndroidNotification();
        notificacion.Title = "FROGGY QUIERE ENTRENAR ";
        notificacion.Text = "¡Vuelve! ¡Hay que ganar esa carrera!";

        // TIEMPO: 10 segundos para probar (luego cambialo a horas)
        notificacion.FireTime = System.DateTime.Now.AddSeconds(10);

        // ICONOS: Usamos los nombres clave que configuraremos en Unity
        notificacion.SmallIcon = "icon_0";
        notificacion.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notificacion, idCanal);
        Debug.Log(" Notificación de Froggy programada para 10 seg.");
    }

    IEnumerator SolicitarPermisos()
    {
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
        {
            yield return null;
        }
    }
}