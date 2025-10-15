// Ready translated locale messages
// The structure of the locale message is the hierarchical object structure with each locale as the top property

const messages = {
    en: {
        common: {
            app_name: "PM Pulse: management news",
            author_name: "Sergey Sidorov",
            created_date: "2025",
            close_title: "Close",
        },
        post: {
            open_post: "Open post",
            post_column_title: "Post",
            post_date_column_title: "Date",
        },
        feed: {
            feed_error_load_title: "Failed to load feed",
            feed_error_load_description: "There was a problem fetching the feed. Please try again later.",
            last_sync_date_title: "Last sync date",
            feed_title: "Feed",
        },
        vuestic: {
            progressState: "",
            select: "Select",
            noOptions: "Empty",
            ok: "OK",
            cancel: "Cancel"
        }
    },
    ru: {
        common: {
            app_name: "PM Pulse: новости в мире менеджмента",
            author_name: "Сергей Сидоров",
            created_date: "2025",
            close_title: "Закрыть",
        },
        vuestic: {
            progressState: "",
            select: "Выбрать",
            noOptions: "Пусто",
            ok: "OK",
            cancel: "Отмена"
        },
        post: {
            open_post: "Открыть пост",
            post_column_title: "Пост",
            post_date_column_title: "Дата",
        },
        feed: {
            feed_error_load_title: "Не удалось загрузить ленту",
            feed_error_load_description: "Произошла ошибка при загрузке ленты. Попробуйте позже.",
            last_sync_date_title: "Обновлено",
            feed_title: "Лента",
        },
    },
}

export default messages;