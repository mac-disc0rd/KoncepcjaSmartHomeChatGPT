using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Iot.V1;
using Grpc.Core;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;


namespace KoncepcjaSmartHomeChatGPT
{
    class Program
    {
        static void Main(string[] args)
        {
            // Pobierz poświadczenia OAuth2.
            GoogleCredential credential = GoogleCredential.GetApplicationDefault();
            // Konfiguruj klienta IoT Core.
            Channel channel = new Channel(IotServiceClient.DefaultEndpoint.Host,
                                            credential.ToChannelCredentials());
            IotServiceClient client = IotServiceClient.Create(channel);
            // Wyświetl listę dostępnych urządzeń.
            ListDevices(client);

            // Utwórz urządzenie IoT Core.
            CreateDeviceRequest createRequest = new CreateDeviceRequest()
            {
                ParentAsRegistryName = RegistryName.FromProjectLocationRegistry("my-project-id",
                                                                                    "us-central1",
                                                                                    "my-registry-id"),
                Device = new Device()
                {
                    Id = "my-device-id",
                    Credentials = { new DeviceCredential() { PublicKey = new PublicKeyCredential() { Format = "RSA_X509_PEM", Key = GetPublicKeyPem() } } }
                }
            };
            client.CreateDevice(createRequest);

            // Przypisz urządzenie do pokoju w systemie.
            AssignDeviceToRoom("my-device-id", "living-room");

            // Uzyskaj dostęp do urządzenia za pomocą protokołu Wi-Fi.
            string wifiDeviceAddress = "192.168.1.100";
            ConnectToDevice(wifiDeviceAddress);

            // Pobierz dane z czujnika temperatury i wyświetl na konsoli.
            float temperature = ReadTemperature();
            Console.WriteLine("Temperature: " + temperature + " C");

            // Wyślij dane do chmury.
            SendDataToCloud("temperature", temperature.ToString());

            // Zakończ połączenie z urządzeniem i zamknij kanał IoT Core.
            DisconnectFromDevice(wifiDeviceAddress);
            channel.ShutdownAsync().Wait();
        }

        static void ListDevices(IotServiceClient client)
        {
            // Pobierz listę urządzeń IoT Core.
            string formattedParent = RegistryName.Format("[PROJECT]", "[LOCATION]", "[REGISTRY]");
            var listDevicesRequest = new ListDevicesRequest
            {
                ParentAsRegistryName = RegistryName.FromString(formattedParent)
            };
            var devices = client.ListDevices(listDevicesRequest);

            // Wyświetl nazwy urządzeń.
            Console.WriteLine("Devices:");
            foreach (var device in devices)
            {
                Console.WriteLine($"{device.Id}");
            }
        }

        static void AssignDeviceToRoom(string deviceId, string roomName)
        {
            // Przypisz urządzenie do pokoju.
            Console.WriteLine("Device " + deviceId + " assigned to room " + roomName);
        }

        static void ConnectToDevice(string deviceAddress)
        {
            // Nawiąż połączenie z urządzeniem za pomocą protokołu Wi-Fi.
            Console.WriteLine("Connected to device at address " + deviceAddress);
        }

        static float ReadTemperature()
        {
            // Odczytaj dane z czujnika temperatur
        }
    }
}