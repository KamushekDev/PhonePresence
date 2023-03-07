using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhonePresenceBot.Core;
using PhonePresenceBot.Infrastructure;


Console.OutputEncoding = System.Text.Encoding.UTF8;

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddJsonFile("appsettings.json", true);
configurationBuilder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
configurationBuilder.AddEnvironmentVariables();
var configuration = configurationBuilder.Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddInfrastructure(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

// var phonePresenceService = serviceProvider.GetRequiredService<IPhonePresenceService>();
//
// var request = new PhonePresenceRequest()
// {
//     PhoneName = "Kamushek-S21-Ultra"
// };
//
// var result = await phonePresenceService.IsPhonePresented(request);

var bot = serviceProvider.GetRequiredService<IPhonePresenceBot>();

await bot.Run(CancellationToken.None);