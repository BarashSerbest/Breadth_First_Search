using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barash_Serbest_5220505051 // İhtiyaç durumu öncelikli dağıtım projesi => Breadth First Search Algoritması 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var graph = new Dictionary<string, Dictionary<string, int>>(); // "graph" adında bir "Dictionary" tanımlama.

            // "graph" sözlüğüne, anahtar olarak lokasyon ismi ve değer olarak o lokasyondaki malzeme miktarı ekleme.
            MalzemeMiktariEkleme(graph);

            // "distances" adında bir boş sözlük oluşturma, bu sözlük, her bir lokasyonun diğer lokasyonlara olan en kısa mesafelerini tutacak.
            Dictionary<string, Dictionary<string, int>> distances = new Dictionary<string, Dictionary<string, int>>();

            foreach (var start in graph.Keys) // Her bir lokasyon için bir alt sözlük oluşturup, bu alt sözlüklere herhangi bir mesafe atama.
            {
                distances.Add(start, new Dictionary<string, int>());
                foreach (var end in graph.Keys)
                {
                    distances[start].Add(end, int.MaxValue);
                }
            }

            foreach (var start in graph.Keys) // Her bir lokasyona, diğer tüm lokasyonlara olan mesafeleri hesaplama, başlangıçta her mesafe sonsuz kabul ediliyor.
            {
                distances[start][start] = 0;
                Queue<string> queue = new Queue<string>();
                queue.EnQueue(start);

                while (queue.Count > 0) // Breadth First Search Algoritması ile lokasyonlardaki ilişkileri graph adındaki sözlükten alma.
                {
                    string current = queue.DeQueue();

                    foreach (var neighbor in graph[current].Keys)
                    {
                        if (distances[start][neighbor] == int.MaxValue)
                        {
                            distances[start][neighbor] = distances[start][current] + graph[current][neighbor];
                            queue.EnQueue(neighbor);
                        }
                    }
                }
            }

            List<string> SaglikMalzemesiLokasyon, TemelGidaLokasyon, IsinmaGereciLokasyon, GiyecekLokasyon; // ihtiyaç durumu listeleri 
            Locations(out SaglikMalzemesiLokasyon, out TemelGidaLokasyon, out IsinmaGereciLokasyon, out GiyecekLokasyon);

            // En kısa mesafeleri depolamak için boş bir dictionary oluşturma.
            Dictionary<string, Dictionary<string, int>> shortestDistances = new Dictionary<string, Dictionary<string, int>>();


            Dictionary<string, Dictionary<string, int>> supplies = new Dictionary<string, Dictionary<string, int>>();

            foreach (var location in graph.Keys)
            {
                supplies[location] = new Dictionary<string, int>();
                supplies[location]["Sağlık Malzemesi"] = 0;
                supplies[location]["Temel Gıda"] = 0;
                supplies[location]["Isınma Gereci"] = 0;
                supplies[location]["Giyecek"] = 0;
            }

            int healthSupplies = 100;
            int foodSupplies = 100;
            int heatingSupplies = 70;
            int clothingSupplies = 70;

            foreach (var start in SaglikMalzemesiLokasyon) // Sağlık Malzemesi Dağıtım Planı
            {
                shortestDistances.Add(start, new Dictionary<string, int>());

                foreach (var end in graph.Keys) // Her Sağlık Malzemesi deposu için, tüm lokasyonlara olan mesafeleri hesaplayarak kısa mesafeleri sözlüğüne ekleme.
                {
                    if (SaglikMalzemesiLokasyon.Contains(end))
                    {
                        shortestDistances[start].Add(end, distances[start][end]);
                    }
                }
            }

            foreach (var start in TemelGidaLokasyon) // Temel Gıda Dağıtım Planı
            {
                int shortestDistance = int.MaxValue;
                string closestLocation = "";

                foreach (var end in TemelGidaLokasyon) // Her Temel Gıda deposu için, tüm lokasyonlara olan mesafeleri hesaplayarak kısa mesafeleri sözlüğüne ekleme.
                {
                    if (shortestDistances[start][end] < shortestDistance)
                    {
                        shortestDistance = shortestDistances[start][end];
                        closestLocation = end;
                    }
                }

                if (foodSupplies > 0)
                {
                    supplies[closestLocation]["Temel Gıda"] += 1;
                    foodSupplies -= 1;
                }
            }

            foreach (var start in IsinmaGereciLokasyon) // Isınma Gereci Dağıtım Planı
            {
                int shortestDistance = int.MaxValue;
                string closestLocation = "";

                foreach (var end in IsinmaGereciLokasyon) // Her Isınma Gereci deposu için, tüm lokasyonlara olan mesafeleri hesaplayarak kısa mesafeleri sözlüğüne ekleme.
                {
                    if (shortestDistances[start][end] < shortestDistance)
                    {
                        shortestDistance = shortestDistances[start][end];
                        closestLocation = end;
                    }
                }

                if (heatingSupplies > 0)
                {
                    supplies[closestLocation]["Isınma Gereci"] += 1;
                    heatingSupplies -= 1;
                }
            }

            foreach (var start in GiyecekLokasyon) // Giyecek Dağıtım Planı
            {
                int shortestDistance = int.MaxValue;
                string closestLocation = "";

                foreach (var end in GiyecekLokasyon) // Her Giyecek deposu için, tüm lokasyonlara olan mesafeleri hesaplayarak kısa mesafeleri sözlüğüne ekleme.
                {
                    if (shortestDistances[start][end] < shortestDistance)
                    {
                        shortestDistance = shortestDistances[start][end];
                        closestLocation = end;
                    }
                }

                if (clothingSupplies > 0)
                {
                    supplies[closestLocation]["Giyecek"] += 1;
                    clothingSupplies -= 1;
                }
            }

            // Dağıtılan malzemelerin toplamını ve dağıtıldığı lokasyonları ekrana yazdırma  
            foreach (var location in graph.Keys)
            {
                Console.WriteLine(location + ":");
                foreach (var supply in supplies[location].Keys)
                {
                    Console.WriteLine("\t" + supply + ": " + supplies[location][supply]);
                }
            }


        }

        private static void MalzemeMiktariEkleme(Dictionary<string, Dictionary<string, int>> graph)
        {
            graph.Add("Lokasyon1", new Dictionary<string, int>() { { "Sağlık Malzemesi", 10 }, { "Temel Gıda", 20 } }); // Her lokasyonun malzeme miktarları da bir alt sözlük olarak ekleniyor.
            graph.Add("Lokasyon2", new Dictionary<string, int>() { { "Isınma Gereci", 15 }, { "Giyecek", 10 } });
            graph.Add("Lokasyon3", new Dictionary<string, int>() { { "Temel Gıda", 10 }, { "Giyecek", 20 } });
            graph.Add("Lokasyon4", new Dictionary<string, int>() { { "Sağlık Malzemesi", 30 }, { "Isınma Gereci", 5 } });
            graph.Add("Lokasyon5", new Dictionary<string, int>() { { "Sağlık Malzemesi", 20 }, { "Temel Gıda", 15 } });
            graph.Add("Lokasyon6", new Dictionary<string, int>() { { "Isınma Gereci", 10 }, { "Giyecek", 10 } });
            graph.Add("Lokasyon7", new Dictionary<string, int>() { { "Sağlık Malzemesi", 10 }, { "Temel Gıda", 5 } });
            graph.Add("Lokasyon8", new Dictionary<string, int>() { { "Giyecek", 10 }, { "Isınma Gereci", 10 } });
            graph.Add("Lokasyon9", new Dictionary<string, int>() { { "Sağlık Malzemesi", 20 }, { "Temel Gıda", 10 } });
            graph.Add("Lokasyon10", new Dictionary<string, int>() { { "Giyecek", 20 }, { "Isınma Gereci", 5 } });
        }

        private static void Locations(out List<string> SaglikMalzemesiLokasyon, out List<string> TemelGidaLokasyon, out List<string> IsinmaGereciLokasyon, out List<string> GiyecekLokasyon)
        {
            SaglikMalzemesiLokasyon = new List<string>()  // Sağlık Malzemesi ihtiyaç durumu listesi
            { "Lokasyon1", "Lokasyon4", "Lokasyon5", "Lokasyon7", "Lokasyon9" };
            TemelGidaLokasyon = new List<string>()        // Temel Gıda ihtiyaç durumu listesi 
            { "Lokasyon1", "Lokasyon4", "Lokasyon5", "Lokasyon7", "Lokasyon9" };
            IsinmaGereciLokasyon = new List<string>()     // Isınma gereci ihtiyaç durumu listesi  
            { "Lokasyon1", "Lokasyon3", "Lokasyon6", "Lokasyon8", "Lokasyon10" };
            GiyecekLokasyon = new List<string>()         // Giyecek ihtiyaç durumu listesi
            { "Lokasyon2", "Lokasyon3", "Lokasyon5", "Lokasyon7", "Lokasyon9" };
        }
    }
}
