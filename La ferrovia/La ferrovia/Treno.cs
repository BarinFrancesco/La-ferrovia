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
        
        public Treno(int id ,SemaphoreSlim semaforo, Ferrovia ferrovia)
        {
            ID = id;
            Direzione = Random.Shared.Next(1, 3) % 2 == 0 ? true : false; //direzione casuale 
            CodaAttesa = semaforo;
            FerroviaDiRiferimento = ferrovia;
        }

        public async Task EntraBinario()
        {
            while (true)
            {
                bool entra = await FerroviaDiRiferimento.PuòEntrare(this);

                if (entra)
                {
                    Interlocked.Increment(ref FerroviaDiRiferimento.TreniInMovimento);
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
            int tempo = Random.Shared.Next(1, 6);
            tempo *= 1000;

            await Task.Delay(tempo);

            Interlocked.Decrement(ref FerroviaDiRiferimento.TreniInMovimento);

            if (Interlocked.Read(ref FerroviaDiRiferimento.TreniInMovimento) == 0)
            {
                FerroviaDiRiferimento.Direzione = null;
                CodaAttesa.Release();
            }
        }

        public async Task Aspetta()
        {
            await CodaAttesa.WaitAsync();

        }


    }
}
