Инфраструктурные паттерны

**Цель:**

В этом ДЗ вы создадите простейший RESTful CRUD.

**Описание/Пошаговая инструкция выполнения домашнего задания:**

Сделать простейший RESTful CRUD по созданию, удалению, просмотру и обновлению пользователей.

Пример API - https://app.swaggerhub.com/apis/otus55/users/1.0.0

Добавить базу данных для приложения.

Конфигурация приложения должна хранится в Configmaps.

Доступы к БД должны храниться в Secrets.

Первоначальные миграции должны быть оформлены в качестве Job-ы, если это требуется.
Ingress-ы должны также вести на url arch.homework/ (как и в прошлом задании)

На выходе должны быть предоставлена

1. ссылка на директорию в github, где находится директория с манифестами кубернетеса
2. инструкция по запуску приложения.

* команда установки БД из helm, вместе с файлом values.yaml.
* команда применения первоначальных миграций
* команда kubectl apply -f, которая запускает в правильном порядке манифесты кубернетеса

3. Postman коллекция, в которой будут представлены примеры запросов к сервису на создание, получение, изменение и удаление пользователя. Важно: в postman коллекции использовать базовый url - arch.homework.
4. Проверить корректность работы приложения используя созданную коллекцию newman run коллекция_постман и приложить скриншот/вывод исполнения корректной работы

*Задание со звездочкой:*

Добавить шаблонизацию приложения в helm чартах

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
* Установлен Newman (npm install -g newman).

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
