using System;
using System.Collections.Generic;
using System.Linq;

namespace LAB2
{
    public delegate void DeviceStatusChange(SmartDevice device);

    public class SmartDevice
    {
        public string deviceName;
        public string type;
        public bool currentStatus;
        public double temperature;
        public bool securityStatus;
        public double temperatureThreshold = 70.0; // Thermostat threshold 
        
        public event DeviceStatusChange StatusChanged;

        public void changeStatus(bool newStatus)
        {
            if (newStatus != currentStatus)
            {
                currentStatus = newStatus;
                StatusChanged?.Invoke(this);
            }
        }

        public void changeTemperature(double newTemperature)
        {
            temperature = newTemperature;
            if (newTemperature > temperatureThreshold)
            {
                StatusChanged?.Invoke(this);
            }
        }

        public void changeSecurityStatus(bool newStatus)
        {
            if (newStatus != securityStatus)
            {
                securityStatus = newStatus;
                StatusChanged?.Invoke(this);
            }
        }
    }

    public class Program
    {
        private static void NotificationHandler(SmartDevice device)
        { 
            Console.WriteLine($"Notification: {device.deviceName} ({device.type}) status changed.");
            if (device.type == "Thermostat" && device.temperature > device.temperatureThreshold)
            {
                Console.WriteLine($"{device.deviceName} ({device.type}) temperature ({device.temperature}) exceeds threshold {device.temperatureThreshold}.");
            }
            else
            {
                Console.WriteLine($" - Status: {device.currentStatus}, Temperature: {device.temperature}, Security: {device.securityStatus}");
            }
        }

        public static void Main(string[] args)
        {
            List<SmartDevice> devices = new List<SmartDevice>
            {
                new SmartDevice { deviceName = "Light", type = "Light", currentStatus = false },
                new SmartDevice { deviceName = "Thermostat", type = "Thermostat", currentStatus = true, temperature = 50.0 },
                new SmartDevice { deviceName = "SecurityCamera", type = "Camera", currentStatus = true, securityStatus = true },
                new SmartDevice { deviceName = "Camera", type = "Camera", currentStatus = true, securityStatus = false },
            };

            foreach (var device in devices)
            {
                device.StatusChanged += NotificationHandler;
            }

            Console.WriteLine("Devices currently turned on:");
            var turnedOnDevices = devices.Where(d => d.currentStatus).ToList();
            turnedOnDevices.ForEach(d => Console.WriteLine($"{d.deviceName} ({d.type})"));

            Console.WriteLine("\nTurning on the light:");
            var light = devices[0];
            light.changeStatus(true);

            Console.WriteLine("\nDevices currently turned on:");
            turnedOnDevices = devices.Where(d => d.currentStatus).ToList();
            turnedOnDevices.ForEach(d => Console.WriteLine($"{d.deviceName} ({d.type})"));

            Console.WriteLine("\nCameras that are security cameras:");
            var securityCameras = from device in devices
                                  where device.type == "Camera" && device.securityStatus
                                  select device;
            foreach (var securityCamera in securityCameras)
            {
                Console.WriteLine($"{securityCamera.deviceName} - Type: {securityCamera.type} - Security: {securityCamera.securityStatus}");
            }

            Console.WriteLine("\nUpdating Thermostat temperature...");
            var thermostat = devices[1];
            thermostat.changeTemperature(80.0);

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }
    }
}