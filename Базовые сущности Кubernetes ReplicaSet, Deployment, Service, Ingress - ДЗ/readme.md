# Домашнее задание: Основы работы с Kubernetes (Часть 2)

Цель:

В этом ДЗ вы научитесь создавать минимальный сервис.

Описание/Пошаговая инструкция выполнения домашнего задания:

**Шаг 1**. Создать минимальный сервис, который

отвечает на порту 8000
имеет http-метод

GET /health/

RESPONSE: {"status": "OK"}

**Шаг 2**. Cобрать локально образ приложения в докер.

Запушить образ в dockerhub

**Шаг 3**. Написать манифесты для деплоя в k8s для этого сервиса.

Манифесты должны описывать сущности: Deployment, Service, Ingress.

В Deployment могут быть указаны Liveness, Readiness пробы.

Количество реплик должно быть не меньше 2. Image контейнера должен быть указан с Dockerhub.

Хост в ингрессе должен быть arch.homework. В итоге после применения манифестов GET запрос на http://arch.homework/health должен отдавать {“status”: “OK”}.

**Шаг 4**. На выходе предоставить

0) ссылку на github c манифестами. Манифесты должны лежать в одной директории, так чтобы можно было их все применить одной командой kubectl apply -f .

1) url, по которому можно будет получить ответ от сервиса (либо тест в postmanе).

Задание со звездой:

В Ingress-е должно быть правило, которое форвардит все запросы с /otusapp/{student name}/* на сервис с rewrite-ом пути. Где {student name} - это имя студента.

Например: curl arch.homework/otusapp/aeugene/health -> рерайт пути на arch.homework/health

**Рекомендации по форме сдачи дз:**

* использовать nginx ingress контроллер, установленный через хелм, а не встроенный в миникубик:
kubectl create namespace m && helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx/ && helm repo update && helm install nginx ingress-nginx/ingress-nginx --namespace m -f nginx-ingress.yaml (файл приложен к занятию)
* https://kubernetes.github.io/ingress-nginx/user-guide/basic-usage/
необходимо в новых версиях nginx добавлять класс ингресса
ingressClassName: nginx
* прикладывать к 2 дз урл для проверки: curl http://arch.homework/health или как указано в дз со *.
К 3 дз и далее дз прикладывать коллекцию postman и проверять ее работу через newman run имя_коллекции
прикладывать кроме команд разворачивания приложения, команду удаления)
* прописать у себя в /etc/hosts хост arch.homework с адресом своего миникубика (minikube ip), чтобы обращение было по имени хоста в запросах, а не айпи
* Обратите внимание, что при сборке на m1 при запуске вашего контейнера на стандартных платформах будет ошибка такого вида:
standard_init_linux.go:228: exec user process caued: exec format error

Для сборки рекомендую указать тип платформы linux/amd64:
docker build --platform linux/amd64 -t tag .

Более подробно можно прочитать в статье: https://programmerah.com/how-to-solve-docker-run-error-standard_init_linux-go219-exec-user-process-caused-exec-format-error-39221/

**Критерии оценки:**

"Принято" - задание выполнено полностью
"Возвращено на доработку" - задание не выполнено полностью

# Проект проверки задания на Windows 10 Pro 22H2, x64

## Предпосылки

* Установлен Docker 26.0.0.
* Установлен Kubernetes (клиент v1.29.2, сервер v1.30.0).
* Установлен Minikube v1.33.0 с драйвером ВМ hyperv.
* Установлен Node.js v18.10.0.
* Установлен Newman (npm install -g newman).

## Шаги

Запустить командную строку (cmd) от имени администратора, перейти в папку "Базовые сущности Кubernetes ReplicaSet, Deployment, Service, Ingress - ДЗ" и выполнить указанные ниже команды.

    docker context use default
    minikube start

Первый раз после установки Minikube вместо команды `minikube start` следует выполнить `minikube start --vm-driver=hyperv`.

    minikube addons enable ingress
    kubectl apply -f k8s\.
    minikube ip

Сделать в файле *hosts* запись `arch.homework` с адресом, полученным с помощью последней команды, например, `1.2.3.4 arch.homework`.

    newman run otus-ms-hw-3-k8s-basics.postman_collection.json

Пример ожидаемого результата:

	newman

	otus-ms-hw-3-k8s-basics

	→ curl arch.homework/otusapp/aeugene/health yields {"status":"OK"}
	  GET arch.homework/otusapp/aeugene/health [200 OK, 170B, 88ms]
	  √  Response status code is 200
	  √  Response time is within an acceptable range
	  √  Content-Type header is application/json
	  √  Status field equals 'OK'
    
Для остановки и удаления кластера Minikube надо выполнить следующее.

    minikube stop
    minikube delete --all
