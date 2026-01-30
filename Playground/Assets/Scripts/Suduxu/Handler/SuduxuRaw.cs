using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class SuduxuRaw
{
    public static Thread serverThread;

    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Server -----------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void start_suduxu();

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool is_running();

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void stop_suduxu();

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void determine_mode();

    // ---------------------------------  Clients  ----------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr find_all_clients();

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr find_client_by_id(ushort id);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void broadcast(IntPtr ptr);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void unicast(ushort id, IntPtr ptr);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void disconnect_client(ushort id);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void disconnect_all();

    // ----------------------------------- State -----------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool get_button_in_state(ushort id, ButtonInputType type, ButtonInputState state);

    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // --------------------------------- Game Loop ----------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool tick(float delta);
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Config -----------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr config();
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr addresses();
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Utils ------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool free(IntPtr data);
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Sensor Event -----------------------------
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SensorEventCallback(IntPtr sensorData);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void register_sensor_event_callback(SensorEventCallback eventCallback);
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Event ------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void register_event_callback(EventCallback eventCallback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr ptr);
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // --------------------------------- Screenshot ---------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void notify_screenshot(string path, ushort id);

    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ------------------------------------- QR -------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern QrResult get_qr_code_rendered();
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_qr_buffer(IntPtr ptr, uint size);
}