using BookManagement.Models;

namespace BookManagement.Services;

/// <summary>
/// Интерфейс сервиса для работы с историями/рассказами.
/// Предоставляет методы для управления историями, избранным и лайками.
/// </summary>
public interface IStoryService
{
    /// <summary>
    /// Асинхронно получает все истории, упорядоченные по дате создания в убывающем порядке.
    /// </summary>
    /// <returns>Список всех историй.</returns>
    Task<List<Story>> GetAllStoriesAsync();

    /// <summary>
    /// Асинхронно получает историю по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор истории.</param>
    /// <returns>Объект <see cref="Story"/> если найден, иначе null.</returns>
    Task<Story?> GetStoryByIdAsync(int id);

    /// <summary>
    /// Асинхронно создаёт новую историю.
    /// Проверяет корректность рейтинга (от 1 до 5).
    /// </summary>
    /// <param name="story">Объект истории для создания.</param>
    /// <returns>Созданный объект истории.</returns>
    /// <exception cref="ArgumentException">Выбросывается, если рейтинг не в диапазоне 1-5.</exception>
    Task<Story> CreateStoryAsync(Story story);

    /// <summary>
    /// Асинхронно обновляет информацию об истории.
    /// Может обновлять только автор истории.
    /// </summary>
    /// <param name="story">Объект истории с обновлёнными данными.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего обновление.</param>
    /// <returns>true если обновление успешно, false если история не найдена или прав нет.</returns>
    Task<bool> UpdateStoryAsync(Story story, string userId);

    /// <summary>
    /// Асинхронно удаляет историю.
    /// Может удалить только автор истории.
    /// </summary>
    /// <param name="storyId">Идентификатор истории для удаления.</param>
    /// <param name="userId">Идентификатор пользователя, запрашивающего удаление.</param>
    /// <returns>true если удаление успешно, false если история не найдена или прав нет.</returns>
    Task<bool> DeleteStoryAsync(int storyId, string userId);

    /// <summary>
    /// Асинхронно проверяет, добавлена ли история в избранное пользователем.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>true если история в избранном, иначе false.</returns>
    Task<bool> IsFavoriteAsync(string userId, int storyId);

    /// <summary>
    /// Асинхронно переключает статус избранной истории.
    /// Добавляет в избранное, если её там нет, и удаляет, если она там есть.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    Task ToggleFavoriteAsync(string userId, int storyId);

    /// <summary>
    /// Асинхронно получает список идентификаторов избранных историй пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <returns>Список идентификаторов избранных историй.</returns>
    Task<List<int>> GetFavoriteStoryIdsAsync(string userId);

    /// <summary>
    /// Асинхронно получает количество лайков или дизлайков для истории.
    /// </summary>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <param name="isLiked">true для подсчёта лайков, false для подсчёта дизлайков.</param>
    /// <returns>Количество лайков или дизлайков.</returns>
    Task<int> GetLikeCountAsync(int storyId, bool isLiked);

    /// <summary>
    /// Асинхронно переключает лайк/дизлайк пользователя для истории.
    /// Если isLiked равно null, удаляет существующую оценку.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <param name="isLiked">true для лайка, false для дизлайка, null для удаления оценки.</param>
    Task ToggleStoryLikeAsync(string userId, int storyId, bool? isLiked);

    /// <summary>
    /// Асинхронно получает оценку пользователя для истории.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="storyId">Идентификатор истории.</param>
    /// <returns>true если это лайк, false если дизлайк, null если оценки нет.</returns>
    Task<bool?> GetUserStoryLikeAsync(string userId, int storyId);
}