using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace La_ferrovia
{
    public class Treno
    {
        public int ID;
        public bool Direzione { get; private set; }
        readonly SemaphoreSlim CodaPropria;
        readonly SemaphoreSlim CodaOpposta;
        private long AttesaCodaPropria;
        private long AttesaCodaOpposta;
        public Ferrovia FerroviaDiRiferimento;
        
        public Treno(int id , Ferrovia ferrovia)
        {
            ID = id;
            Direzione = Random.Shared.Next(1, 7) % 2 == 0 ? true : false; //direzione casuale 

            CodaPropria = Direzione? ferrovia.CodaAttesaNord : ferrovia.CodaAttesaSud;
            AttesaCodaPropria = Direzione ? ref ferrovia.TreniInAttesaNord : ref ferrovia.TreniInAttesaSud;

            CodaOpposta = Direzione ? ferrovia.CodaAttesaSud : ferrovia.CodaAttesaNord;
            AttesaCodaOpposta = Direzione ? ref ferrovia.TreniInAttesaSud : ref ferrovia.TreniInAttesaNord;
            
            FerroviaDiRiferimento = ferrovia;
        }

        public async Task EntraBinario()
        {
            string direzione = Direzione ? "nord" : "sud";
            Print($"Il treno {ID}, Direzione:{direzione} sta provando ad entrare", 4);
            
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

            Console.WriteLine($"Il treno {ID} esce dal binario");
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


                if (Interlocked.Read(ref AttesaCodaOpposta) != 0)
                {
                    Interlocked.Decrement(ref AttesaCodaOpposta);
                    CodaOpposta.Release();
                } else
                {
                    CodaPropria.Release();
                }
                
            }

            
        }

        public async Task Aspetta()
        {
            string direzione = Direzione ? "nord" : "sud";
            Print($"Il treno {ID}, Direzione:{direzione} sta Aspettando il suo turno......", 3);
            Interlocked.Increment(ref AttesaCodaPropria);
            await CodaPropria.WaitAsync();

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
