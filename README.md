# Основное задание:
1. Account URL: http://localhost:5000/swagger/index.html
2. Hospital URL: http://localhost:5001/swagger/index.html
3. Timetable URL: http://localhost:5002/swagger/index.html
4. Document URL: http://localhost:5003/swagger/index.html

# Примечания
У пользователей Id представлен Guid, а не int, поэтому везде где используется Id пользователей надо передавать строку.

В AccountController был добавлен метод получения пользователя по Id, чтобы можно было выполнить проверку pacientId в DocumentService.

JWT токен надо добавлять через правый верхний блок Authorize в Swagger в следующем виде: Bearer[пробел][токен]
