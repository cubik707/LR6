using Microsoft.Data.SqlClient;
using System;
using System.Data.SqlTypes;
using System.Threading.Tasks;

namespace lab6
{
    internal class Program
    {
        static string connectionString = "Server=WIZ15\\SQLEXPRESS;Database=lab6;Trusted_Connection=True;TrustServerCertificate=True;";

        static async Task Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Подключено");

                bool state = true;
                do
                {
                    Console.WriteLine("Меню");
                    Console.WriteLine("1 - Добавить книгу");
                    Console.WriteLine("2 - Редактировать книгу");
                    Console.WriteLine("3 - Удалить книгу");
                    Console.WriteLine("4 - Вывести все книги");
                    Console.WriteLine("0 - Выход");

                    int choice = -1;
                    try
                    {
                        choice = int.Parse(Console.ReadLine());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    switch (choice)
                    {
                        case 0:
                            state = false;
                            break;
                        case 1:
                            await AddBook(connection);
                            break;
                        case 2:
                            await EditBook(connection);
                            break;
                        case 3:
                            await DeleteBook(connection);
                            break;
                        case 4:
                            await DisplayAllBooks(connection);
                            break;
                        default:
                            Console.WriteLine("Неверный выбор, попробуйте снова.");
                            break;
                    }
                } while (state);
            }
        }

        private static async Task AddBook(SqlConnection connection)
        {
            Console.WriteLine("Введите название книги:");
            string title = Console.ReadLine();

            Console.WriteLine("Введите имя автора:");
            string author = Console.ReadLine();

            Console.WriteLine("Введите жанр:");
            string genre = Console.ReadLine();

            string sql = $"INSERT INTO Books (Title, Author, Genre) VALUES (@Title, @Author, @Genre)";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@Genre", genre);

                await command.ExecuteNonQueryAsync();
                Console.WriteLine("Книга добавлена.");
            }
        }

        private static async Task EditBook(SqlConnection connection)
        {
            Console.WriteLine("Введите ID книги для редактирования:");
            int bookId = -1;

            while (true)
            {
                try
                {
                    bookId = int.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Введите новое название книги:");
            string title = Console.ReadLine();

            Console.WriteLine("Введите новое имя автора:");
            string author = Console.ReadLine();

            Console.WriteLine("Введите новый жанр:");
            string genre = Console.ReadLine();
            string sql = $"UPDATE Books SET Title = @Title, Author = @Author, Genre = @Genre WHERE Book_id = @BookId";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@Genre", genre);
                command.Parameters.AddWithValue("@BookId", bookId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Книга отредактирована.");
                }
                else
                {
                    Console.WriteLine("Книга не найдена.");
                }
            }
        }

        private static async Task DeleteBook(SqlConnection connection)
        {
            Console.WriteLine("Введите ID книги для удаления:");
            int bookId = -1;

            while (true)
            {
                try
                {
                    bookId = int.Parse(Console.ReadLine());
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            string sql = $"DELETE FROM Books WHERE Book_id = @BookId";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@BookId", bookId);
                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Книга удалена.");
                }
                else
                {
                    Console.WriteLine("Книга не найдена.");
                }
            }
        }

        private static async Task DisplayAllBooks(SqlConnection connection)
        {
            string sql = "SELECT * FROM Books";
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    Console.WriteLine("Список книг:");
                    while (await reader.ReadAsync())
                    {
                        int bookId = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string author = reader.GetString(2);
                        string genre = reader.GetString(3);
                        Console.WriteLine($"ID: {bookId}, Название: {title}, Автор: {author}, Жанр: {genre}");
                    }
                }
            }
        }
    }
}