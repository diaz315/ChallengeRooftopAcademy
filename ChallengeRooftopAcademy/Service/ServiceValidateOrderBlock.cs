using ChallengeRooftopAcademy.Models;
using ChallengeRooftopAcademy.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeRooftopAcademy.Service
{
    public class ServiceValidateOrderBlock : IServiceValidateOrderBlock
    {
        public readonly IHttpService _rooftopService;
        public readonly IServiceCache _serviceCache;

        public Dictionary<string, string> dictionaryCheck = new Dictionary<string, string>();

        public ServiceValidateOrderBlock(IHttpService rooftopService, IServiceCache serviceCache)
        {
            _rooftopService = rooftopService;
            _serviceCache = serviceCache;
        }

        public async Task<List<string>> check(List<string> blocksData, string token)
        {
            try
            {
                List<string> resultado = new List<string>();

                Dictionary<string, string> dictionaryCheck = new Dictionary<string, string>();

                dictionaryCheck = blocksData.ToDictionary(x => x);

                bool flagIteration = true;

                string flagBlockValue = blocksData[0];
                resultado.Add(flagBlockValue);
                dictionaryCheck.Remove(flagBlockValue);

                int counter = 0;

                while (flagIteration)
                {
                    foreach (var item in dictionaryCheck)
                    {

                        //Como el ultimo, no lo comparamos , ya que no hay mas con quien hacerlo y ahorramos una consulta del api
                        if (dictionaryCheck.Count == 1)
                        {
                            resultado.Add(item.Value);
                            flagIteration = false;
                            break;
                        }

                        counter++;

                        var responseCheck = new ResponseCheck
                        {
                            blocks = new List<string> { flagBlockValue, item.Value }, //->Generando pareja de bloques a consultar
                            merged = flagBlockValue + item.Value //-->Key para guardar en cache
                        };

                        var result = await _rooftopService.getCheck(responseCheck);

                        //si no pasa por la api sino por cache  elimina cantidad de veces de la consulta
                        if (counter>0 && !result.consultedToApi) {
                            counter -= 1;
                        }

                        if (result != null && result.message)
                        {
                            resultado.Add(item.Value);
                            flagBlockValue = item.Value;
                            dictionaryCheck.Remove(item.Key);
                            break;
                        }
                        //Codigo de seguridad, para el maximo de permutaciones permitidas
                        if (counter == ((blocksData.Count) * (blocksData.Count)))
                        {
                            Console.WriteLine("No se ha encontrado coincidencias");
                            return new List<string>();
                        };
                    }
                }

                //Concatenando lista de valores
                string key = string.Join("", blocksData);
                var finnalCompare = await _rooftopService.checkAllChain(resultado, key);

                var message = finnalCompare.message ? "Correcto" : "Incorrecto";

                Console.WriteLine($"Resultado: {message}");
                Console.WriteLine($"Total Api request: {counter}");

                if (!finnalCompare.message)
                    return new List<string>();

                return resultado;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<string>> executeCheck(string email)
        {
            try
            {
                //Validando formato email
                new MailAddress(email);
                //get token
                await _rooftopService.getToken(email);

                //get blocks
                var blocks = await _rooftopService.getBlocks();

                //get token in cache to send
                var responseTokenCache = _serviceCache.get<ResponseToken>("token");

                //result
                List<string> blocksList = new List<string>();

                //if is not empty iterate
                if (blocks.data.Any())
                {

                    //check all the chains to avoid iterate if the result is correct

                    string key = string.Join("", blocks.data);

                    var resultcheck = await _rooftopService.checkAllChain(blocks.data, key);
                    
                    //Pseudo false
                    resultcheck.message = false;

                    if (resultcheck.message)
                    {
                        Console.WriteLine("__Consulta Directa de Cache__");
                        blocksList = resultcheck.blocks;
                    }
                    else {
                        blocksList = await check(blocks.data, responseTokenCache.token);
                    }

                    int counter = 1;
                    Console.WriteLine("");
                    Console.WriteLine("Imprimiendo resultados:");
                    Console.WriteLine("");
                    foreach (var block in blocksList)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(counter++);
                        sb.Append(" " + block.ToString());

                        Console.WriteLine(sb);
                    }
                }

                return blocksList;

            }
            catch (Exception ex) {
                throw ex;
            }            
        }
    }
}
