Prometheus. Grafana

**Цель:**

В этом ДЗ вы научитесь инструментировать сервис.

**Описание/Пошаговая инструкция выполнения домашнего задания:**

Инструментировать сервис из прошлого задания метриками в формате Prometheus с помощью библиотеки для вашего фреймворка и ЯП.

Сделать дашборд в Графане, в котором были бы метрики с разбивкой по API методам:

1. Latency (response time) с квантилями по 0.5, 0.95, 0.99, max
2. RPS
3. Error Rate - количество 500ых ответов

Добавить в дашборд графики с метрикам в целом по сервису, взятые с nginx-ingress-controller:

4. Latency (response time) с квантилями по 0.5, 0.95, 0.99, max
5. RPS
6. Error Rate - количество 500ых ответов

Настроить алертинг в графане на Error Rate и Latency.

На выходе должно быть:

0) скриншоты дашборды с графиками в момент стресс-тестирования сервиса. Например, после 5-10 минут нагрузки.

1) json-дашборды.

Задание со звездочкой

Используя существующие системные метрики из кубернетеса, добавить на дашборд графики с метриками:

7. Потребление подами приложения памяти
8. Потребление подами приолжения CPU

Инструментировать базу данных с помощью экспортера для prometheus для этой БД.

Добавить в общий дашборд графики с метриками работы БД.

Альтернативное задание (если не хочется самому ставить prometheus в minikube) - https://www.katacoda.com/schetinnikov/scenarios/prometheus-client

**Критерии оценки:**

"Принято" - задание выполнено полностью
"Возвращено на доработку" - задание не выполнено полностью


# Проект проверки задания на Windows 10 Pro 22H2, x64

## Предпосылки

* Установлен Docker 26.0.0.
* Установлен Kubernetes (клиент v1.29.2, сервер v1.30.0).
* Установлен Minikube v1.33.0 с драйвером ВМ hyperv.
* Установлен Helm v3.14.4.
* Установлен Node.js v18.10.0.
* Установлен Newman (`npm install -g newman`).

## Шаги

Запустить командную строку (cmd) от имени администратора, перейти в папку "ДЗ4 - Работа с Helm-ом" и выполнить указанные ниже команды.

    docker context use default
    minikube start
	minikube update-context

Первый раз после установки Minikube вместо команды `minikube start` следует выполнить `minikube start --driver=hyperv`.

	minikube ip

Сделать в файле *hosts* запись `arch.homework` с адресом, полученным с помощью последней команды (`minikube ip`), например, `1.2.3.4 arch.homework`.

    helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx/ && helm repo update && helm install nginx ingress-nginx/ingress-nginx --namespace default -f nginx-ingress.yaml
	helm install ht otus-ms-hw-4
	
Чтобы приложение было полностью работоспособным, надо дождаться завершения Job, подготавливающего базу данных (`kubectl get job`). То есть у Job `ht-db-migration-0` должно быть состояние `Complete` и параметр `Completions` равен `1/1`.

    kubectl apply -f k8s\.
	
Перейти в папку postman и запустить тесты:

	newman run otus-ms-hw-4-helm.postman_collection.json --environment Otus_MS.postman_environment.json
	
Пример ожидаемого результата:	
	
	newman

	otus-ms-hw-4-helm

	→ health probe yields {"status":"OK"}
	  GET arch.homework/otusapp/aeugene/health [200 OK, 170B, 66ms]
	  √  Response status code is 200
	  √  Response time is within an acceptable range
	  √  Content-Type header is application/json
	  √  Status field equals 'OK'

	→ post user
	  POST arch.homework/otusapp/aeugene/api/v1/user [201 Created, 196B, 36ms]
	  √  Response status code is 201
	  √  Response time is within an acceptable range
	  √  Response has header 'Location'
	  √  Validate the response schema for required fields

	→ get posted user
	  GET arch.homework/otusapp/aeugene/api/v1/user/25 [200 OK, 274B, 20ms]
	  √  Response status code is 200
	  √  Response time is less than 200ms
	  √  Validate the response schema for required fields
	  √  Validate the response data

	→ update user
	  PUT arch.homework/otusapp/aeugene/api/v1/user/25 [200 OK, 99B, 25ms]
	  √  Response status code is 200
	  √  Response time is less than 200ms

	→ get updated user
	  GET arch.homework/otusapp/aeugene/api/v1/user/25 [200 OK, 298B, 25ms]
	  √  Response status code is 200
	  √  Response time is less than 200ms
	  √  Validate the response schema for required fields
	  √  Validate the response data

	→ delete user
	  DELETE arch.homework/otusapp/aeugene/api/v1/user/25 [200 OK, 99B, 34ms]
	  √  Response status code is 200
	  √  Response time is less than 200ms

Для удаления приложения и Helm chart с базой данных надо вернуться в папку "ДЗ4 - Работа с Helm-ом" и выполнить следующее.

	kubectl delete -f k8s\.
	helm uninstall ht

Также стоит удалить соответствующие Persistance Volume Claim (`kubectl delete pvc`) и Persistance Volume (`kubectl delete pv`), если они больше не нужны. Для остановки и удаления кластера Minikube надо выполнить следующее.

    minikube stop
    minikube delete --all
