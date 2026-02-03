using System;

namespace La_ferrovia
{
    internal class Program
    {
        static  async Task Main(string[] args)
        {
            Ferrovia Stazione = new Ferrovia();

            Random rnd = new Random();
            List<Task> treni= new();

            for (int i = 0; i < 20; i++)
            {
                int direzione = rnd.Next(2);    // nord → locale  sud → ospite
                treni.Add(new Treno(i+1, Stazione).EntraBinario());
                await Task.Delay(rnd.Next(300, 800));
            }

            
            await Task.WhenAll(treni);

            Console.WriteLine("Simulazione terminata");

        }
    }
}