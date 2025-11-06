# ShortUrl

Breve progetto di esempio per un servizio di URL shortening (redirect) con API di gestione.

Caratteristiche principali

- Servizio Web (`ShortUrl.WebApp`): espone due tipi di endpoint:
 - Redirect pubblico: `GET /{shortUrl}` — risolve il `shortCode` e fa redirect verso la `LongUrl`.
 - API di gestione sotto `/api/v1/` protetta da API key: `ping`, `create`, `edit`.
- Persistenza (`ShortUrl.DbPersistence`): Entity Framework Core con entità per ApiKeys, ShortUrls e ClickEvents.
- Applicazione di supporto (`ShortUrl.DevSupport`): app console usata per applicare le migrazioni EF e per eseguire un seed di sviluppo.
- Host distribuito (`ShortUrl.AppHost`): definisce i servizi locali (Postgres, Redis) e i progetti da avviare in ambiente di sviluppo.

Seed di sviluppo

Il progetto `ShortUrl.DevSupport` aggiunge due API key d'esempio quando eseguito con l'argomento `seed`:
- `api-key-local` (permessi base)
- `api-key-local-custom-url` (permesso di impostare short code personalizzati)

Esempi di comandi utili

- Creare una migration (eseguito dalla cartella `src`):

 `dotnet ef migrations add <MigrationName> --project ShortUrl.DbPersistence --startup-project ShortUrl.DevSupport`

- Applicare migrazioni e seed con l'app di supporto (da cartella `src`):

 `dotnet run --project ShortUrl.DevSupport -- migrate seed`

- Avviare il servizio Web (da cartella `src/ShortUrl.WebApp` oppure da `src` con `--project`):

 `dotnet run --project ShortUrl.WebApp`

- Avviare l'host di sviluppo (se disponibile, avvia Postgres/Redis e i progetti definiti):

 `dotnet run --project ShortUrl.AppHost`

Punti importanti

- La route di redirect legge alcune informazioni della richiesta (IP, User-Agent, Referer) e scrive un evento di click in un canale per l'elaborazione asincrona.
- Le API di gestione usano validazione (`FluentValidation`) e richiedono la chiave API presente nella tabella `ApiKeys` per operazioni protette.

