# Anleitung Setup Github Action Runner in Docker

* Docker starten

* CMD Line öffnen

* Ins Verzeichnis navigieren wo das Dockerfile liegt
  * z.B. cd ....../_GitHub_runner

* Repo URL und Token im entrypoint kontrollieren

* Image bauen (sprechenden Namen vergeben)
  * __docker build -t github-runner .__
  * alternativ bei Problemen mit entrypoint.sh file --> __docker build --no-cache -t github-runner .__

Sobald Build abgeschlossen ist, kann der Container gestartet werden.

* via command: __docker run --name *name z.B. snack-runner* -e REPO_URL=*github-url* -e RUNNER_TOKEN=*github-token* github-runner__


# Integration Test
Für Integration Tests (End-to-End) muss der Runner eine Verbindung zum DB-Container haben.
Die beiden in ein gemeinsames Netzwerk hinzufügen.

### Docker
* __docker create network *name z.B. dev-network*__
* __docker connect *dev-network* *snack-runner*__
* __docker connect *dev-network* *snack-db*__

### C#
Änderungen im appsettings.json
* Anpassen des Conncetion strings
* von "ConnectionString": "Host=__*localhost*__;......"
* zu  "ConnectionString": "Host=__*snack_db*__;......"
