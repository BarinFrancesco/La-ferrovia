using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_ferrovia
{
    public class Ferrovia
    {

        public SemaphoreSlim CodaAttesa;
        public bool? Direzione;

        public long TreniConsecutivi;
        private int maxcount;
        public long TreniInMovimento;
        public long TreniInAttesaDestra;
        public long TreniInAttesaSinistra;

        public Ferrovia()
        {
            CodaAttesa = new SemaphoreSlim(0);
            TreniInMovimento = 0;
            TreniConsecutivi = 0;
            TreniInAttesaDestra = 0;
            TreniInAttesaSinistra = 0;
            Direzione = null;
            maxcount = 3;
        }

        public Task<bool> PuòEntrare(Treno treno)
        {
            if(Direzione == treno.Direzione || Direzione == null)
            {

                if(Interlocked.Read(ref TreniConsecutivi) < maxcount)
                {
                    Interlocked.Increment(ref TreniConsecutivi);
                    return Task.FromResult(true);
                } else
                {
                    Direzione = !Direzione;
                    return Task.FromResult(false);
                }
                
            } else
            {
                return Task.FromResult(false);
            }
            
            
        }
    }
}
