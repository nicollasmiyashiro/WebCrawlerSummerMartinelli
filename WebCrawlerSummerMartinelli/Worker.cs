using Microsoft.Extensions.Hosting;

namespace WebCrawlerSummerMartinelli;

public class Worker(Summer summer) : BackgroundService
{
	private readonly Summer _summer = summer;

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_summer.VerifyIfHasEvent();
			await Task.Delay(600000, cancellationToken);
		}
	}
}

#region CodigoJS
/*
document.getElementsByClassName('@container/links-container')[0].childNodes.forEach((n) => {
	let sympla = n.getElementsByTagName('a')[0];
	let linkEletroHits = textoTemSummerEletroHits(sympla.href);
	let textoEletroHits = textoTemSummerEletroHits(sympla.getElementsByTagName('p')[0].outerText);
	console.log(textoEletroHits || linkEletroHits);
})

function textoTemSummerEletroHits(texto){
	let textoEmLowerCase = texto.toLowerCase();
	return textoEmLowerCase.includes(['summer', 'eletro', 'hits']);
}
*/
#endregion