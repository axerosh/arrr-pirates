# TDDD57 ARRR Pirates! Interaktionsreflektion

Erik Mansén, erima668 - Axel Nordanskog, axeno840

## Inledning
Vårt projekt baserades till att börja med på idén att skapa ett plan av vatten upphöjt ovanför spelytan var spelarna då är tvugna att röra sin mobil i den fysiska världen för att skifta mellan att se vad som sker ovanför vattenytan respektive under den. Vi tänkte att detta skulle vara ett intressant sätt att få in den fysiska interaktion som kursens titel talar om.

Därifrån kom idén att låta spelet handla om ett skepp med en besättning som ska kontrolleras för att interagera med föremål på havets botten. Detta tog till slut formed av ett slags rts-spel var en "spelplan" kan placeras på en yta som ett AR-objekt. Denna spelplan är ett 1x1m<sup>2</sup> tvärsnitt av en större spelvärld som alltid hålls centrerad på spelarens skepp. På skeppet finns 3 besättningsmän, 2 dykare och en styrman, samt en kompass som pekar ut var närmsta skattkista finns att hitta. Spelet går ut på att använda dessa besättningsmän för att hämta 10 skattkistor från havets botten.

## Interaktion

#### Kamerastyrning
Navigation genom att fysiskt flytta och rotera sin telefon fungerade över lag bra.
Som användare var det roligt att cinematiskt panorera kameran längs båten efter behag och man fick också möjlighet att styra båten från valfritt perspekiv.
Kamerans virtuella position var dock alltid lite skakig vilket märktes tydligt när man flyttade den mycket nära saker.
Att man kunde flytta kameran fritt nära saker gjorde även att man kunde klippa igenom modeller efter behag.
Denna typ av upplevse-brytande handlingar var dock inte ett problem för ett goofy litet AR-spel som detta.

#### Skalbar klick-hitbox
Vi hade AR-ovana testanvändare som inte tänkte på att man fysiskt kunde röra kameran närmare för att zooma utan försökte istället fibrilt klicka på besättningsmän på avstånd med flera missar och någon enstaka träff.
Det tycktes dels vara frustrerande och gjorde även att tutorial-prompts lätt missades.
För att motverka detta fibrila klickande har vi låtit klick-hitboxen för saker skala linjärt med avståndet till kameran (med en undre och en övre gräns).
Man kan på så sätt med tillit klicka på saker även från någon meter eller två bort utan att ristera att missa men åndå ha kvar precisionen då kameran är nära plcerad.

#### Styrman och motion controls
Om man klickar på styrman får man tillgång till att styra båten genom motion controls genom att rotera telefonen i samma plan i vilket den ligger (som en ratt).
Att denna interaktion liknar något för många så bekant som en ratt gör den lätt att förstå och det känns mycket responsivt och (förvånandsvärt) bekvämt att styra så.
Vi stötte dock här på problem med att skärmen i Android vändes när vi vände telefonen vilket gjorde styrning mycket obekvämt.
Det hanterade vi genom att bara tillåta de två liggande orienteringarna.
Att ändå ha två, snarare än en, orienteringar låter användaren hålla telefonen åt det håll (höger/vänster) som personligen känns bäst eller som passar situationen bäst om telefonen exempelvis än sladdansluten till höger eller vänster.
(Man laddar nog sällan mobilen medan man vill kunna flytta den i AR men vid utveckling var detta mycket smidigt)
Det gör dock att telefonen inte kan roteras upp till 180 grader utan att skärmen vänds, men man svänger bra nog ändå och lär sig också fort att inte rotera så mycket.

#### Klicka Diver

#### Klicka skatt


## Feedback
#### Placering av spelplan
Vårt spel ska ses nästan som ett brädspel. Man placerar ut sin spelyta i en 3D-miljö och interagerar sedan med ytan i fråga. När appen öppnas kommer spelaren till en skärm som dels visar en informationstext som berättar hur man hittar en bra yta att placera sitt spel på, och dels har en knapp uppe i vänstra hörnet som tar en till en skärm med noggrannare beskrivningar. Denna skärm var tänkt att ha en del hjälpsamma bilder men dessa lades till slut aldrig till då spelare sällan brydde sig om knappen utan istället direkt följde anvisningarna på skärmen.

När ARCore hittat en lämplig yta ritas denna ut på skärmen och en ny hjälptext visas för att förklara hur spelaren kan placera ut spelytan. Denna nya hjälptext tar den förras plats och är av samma färg och nästan lika lång vilket gör det svårt att se att det är en ny text. Detta feedback-bekymmer löstes för liknande texter gällande vad olika besättningsmän används till och samma lösning borde antagligen användas här.

#### Tutorial
För att lära en ny spelare hur spelet spelas använder vi oss dels av text som svävar ovanför besättningsmän (med texten "CLICK ME") och hjälpmedelanden i gränssnittet som förklarar vad en besättningsman kan användas till när den väl är vald. Hjälptexten flash:ar i blått när den byter för att göra det tydligare att en ändring skett.

#### Undervattenseffekter
För att förstärka undervattenskänslan när mobilen rör sig under vattnet implementerade vi en blå ton som läggs på kameran då spelaren befinner sig under vatten. Detta tillför först och främst en starkare inlevelsekänsla då man förväntar sig att ens syn ska fungera annorlunda under vattenytan. Men kan också bidra till att förtydliga att det finns en skillnad mellan att vara ovan vattnet, var man i första hand försöker kontrollera sina besättningsmän, och under vattnet, var man letar efter skattkistor.

#### Val av karaktär
När en besättningsman väljs sker två saker:
* En text högst upp på skärmen visar vad för typ av karaktär man har vald för tillfället. Detta låter spelaren veta vad för konsekvenser ens handlingar kommer ha även om man inte ser den karaktär man valt direkt på skärmen. T.ex kommer ett klick på en skattkista skicka en vald dykare för att hämta den, men kommer inte ha någon som helst effekt om man har styrmannen vald.
* En klammer (dvs. tecknen "[ ]") dyker upp runt karaktären. Detta låter en spelare se konkret vilken karaktär de har valda för stunden.

Båda dessa effekter tillsammans gör det tydligt för spelaren att en karaktär blivit vald när man rört vid den.

En utökning av denna feedback som kunde vara användbar är att ge spelaren någon form av indikation över vart ens valda besättningsman befinner sig när den inte längre syns på skärmen, t.ex. i form av en pil eller liknande.

#### Svallvågor
När styrmannen är vald börjar skeppet röra sig frammåt. Skeppets rörelse visualiseras genom att svallvågor börjar avges från skeppets akter. Denna effekt syns både ovan och under vattnet och låter på så vis spelaren se att skeppet just nu är i rörelse.

#### Brädor i vattnet
Svallvågorna i sig visade sig dock inte vara ett tillräckligt tydligt tecken på rörelse. Flera av de människor vi hade att testa spelet verkade inte uppfatta att skeppet började röra sig när styrmannen valdes vilket ledde till förvirring. För att lösa detta implementerade vi en samling plankor som slumpas ut runt om i spelet, både flytandes på havsytan och liggandes på botten. Dessa gör det mycket tydligare när skeppet är i rörelse då spelaren alltid har någon form av referenspunkt att jämföra med.
