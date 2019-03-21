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
Denna typ av upplevse-brytande handlingar var dock ett problem för ett detta goofy litet AR-spel som detta.

#### Skalbar klick-hitbox
Vi hade AR-ovana testanvändare som inte tänkte på att man fysiskt kunde röra kameran närmare för att zooma utan försökte istället fibrilt klicka på besättningsmän på avstånd med flera missar och någon enstaka träff.
Det tycktes dels vara frustrerande och gjorde även att tutorial-prompts lätt missades.
För att motverka detta fibrila klickande har vi låtit klick-hitboxen för saker skala linjärt med avståndet till kameran (med en undre och en övre gräns).
Man kan på så sätt med tillit klicka på saker även från någon meter eller två bort utan att ristera att missa men åndå ha kvar precisionen då kameran är nära plcerad.

#### Klicka Styrman

#### Motion controlls

#### Klicka Diver

#### Klicka skatt


## Feedback

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
