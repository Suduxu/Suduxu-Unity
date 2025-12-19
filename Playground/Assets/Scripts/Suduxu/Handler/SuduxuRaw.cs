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
    // ----------------------------------- Utils ------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool free(IntPtr data);
    // ------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------
    // ----------------------------------- Sensor Event -----------------------------
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SensorEventCallback(ref SensorDataRaw data);

    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void register_sensor_event_callback(SensorEventCallback eventCallback);


    // ----------------------------------- Event ------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void register_event_callback(EventCallback eventCallback);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventCallback(IntPtr ptr);

    // ----------------------------------- Event ------------------------------------
    [DllImport("suduxu", CallingConvention = CallingConvention.Cdecl)]
    public static extern void log_test();
}