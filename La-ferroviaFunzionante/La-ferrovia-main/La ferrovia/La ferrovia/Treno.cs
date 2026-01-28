using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_ferrovia
{
    public class Treno
    {
        public int ID;
        public bool Direzione { get; private set; }
        readonly SemaphoreSlim CodaAttesa;
        public Ferrovia FerroviaDiRiferimento;
        
        public Treno(int id , Ferrovia ferrovia)
        {
            ID = id;
            Direzione = Random.Shared.Next(1, 7) % 2 == 0 ? true : false; //direzione casuale 
            CodaAttesa = ferrovia.CodaAttesa;
            FerroviaDiRiferimento = ferrovia;
        }

        public async Task EntraBinario()
        {
            string direzione = Direzione ? "nord" : "sud";
            Console.WriteLine($"Il treno {ID}, Direzione:{direzione} sta provando ad entrare");
            while (true)
            {
                bool entra = await FerroviaDiRiferimento.PuòEntrare(this);

                if (entra)
                {
                    FerroviaDiRiferimento.Direzione = Direzione;
                    Interlocked.Increment(ref FerroviaDiRiferimento.TreniInMovimento);
                    await AttraversaBinari();
                    break;
                }
                else
                {
                    await Aspetta();
                }
                await Task.Delay(50);
            }
             
        }


        public async Task AttraversaBinari()
        {
            string direzione = Direzione ? "nord" : "sud";
            Print($"Il treno {ID}, Direzione:{direzione} sta Attraversando i binari....", 2);
            int tempo = Random.Shared.Next(1, 11);
            tempo *= 500;

            await Task.Delay(tempo);

            await LiberaBinari();
        }

        public async Task LiberaBinari()
        {
            string direzione = Direzione ? "nord" : "sud";
            Print($"Il treno {ID}, Direzione:{direzione} Ha finito di attraversare i binari", 1);
            Interlocked.Decrement(ref FerroviaDiRiferimento.TreniInMovimento);

            if (Interlocked.Read(ref FerroviaDiRiferimento.TreniInMovimento) == 0)
            {
                Interlocked.Exchange(ref FerroviaDiRiferimento.TreniConsecutivi, 0);
                Print("Binario libero", 4);
                FerroviaDiRiferimento.Direzione = null;
                CodaAttesa.Release();
            }
        }

        public async Task Aspetta()
        {
            string direzione = Direzione ? "nord" : "sud";
            Print($"Il treno {ID}, Direzione:{direzione} sta Aspettando il suo turno......", 3);
            await CodaAttesa.WaitAsync();

        }

        private void Print(string message, int colore )
        {
            switch (colore)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
