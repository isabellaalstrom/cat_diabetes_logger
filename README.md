# 🐱 Cat Diabetes Logger

Ett Home Assistant Addon för att logga och övervaka diabetes-behandling med Senvelgo för katter. Byggt med ASP.NET Core Razor Pages och Entity Framework Core.

## 📋 Beskrivning

Cat Diabetes Logger är en webbapplikation speciellt designad för att logga och följa kattens diabetesbehandling. Applikationen gör det enkelt att registrera medicin, mätvärden, allmäntillstånd och skapar automatiskt dagliga sammanfattningar.

### Huvudfunktioner

- 🩺 **Medicin-loggning**: Registrera Senvelgo-doser med tidsstämpel och intag
- 📊 **Mätvärden**: Spåra blodglukos, ketoner och vikt
- 😺 **Allmäntillstånd**: Logga aptit, törst och allmänt välbefinnande  
- 🚽 **Eliminering**: Övervaka kiss, bajs och kräkningar
- 📈 **Dagliga sammanfattningar**: Automatiska sammanställningar per dag med min/max/medelvärden
- 🏠 **Home Assistant-integration**: Skickar automatiskt data till Home Assistant sensors och events
- 📱 **Mobilanpassad**: Responsiv design optimerad för mobil användning
- 🗑️ **Redigering**: Ta bort felaktiga loggar enkelt

## 🚀 Installation

### Som Home Assistant Addon

1. Lägg till repository i Home Assistant
2. Installera "Cat Diabetes Logger" addon
3. Starta addonen
4. Öppna webgränssnittet via Home Assistant

### Lokal Utveckling