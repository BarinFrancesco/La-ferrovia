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

        public long TreniInMovimento;
        public long TreniInAttesa;

        public Ferrovia()
        {
            CodaAttesa = new SemaphoreSlim(0);
            TreniInMovimento = 0;
            TreniInAttesa = 0;
            Direzione = null;
        }

        public Task<bool> PuòEntrare(Treno treno)
        {
            if(Direzione == treno.Direzione || Direzione == null)
            {
                return Task.FromResult(true);
            } else
            {
                return Task.FromResult(false);
            }
            
            
        }
    }
}
