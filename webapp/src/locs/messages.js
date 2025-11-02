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
        about: {
            title: "About PmPulse",
            subtitle: "A modern web service for aggregating and organizing news",
            pet_project: "🐶 This is a pet-project",
            overview_title: "Overview",
            overview_description: "PmPulse helps you stay informed without the noise. Instead of subscribing to hundreds of media accounts and dealing with constant notifications, PmPulse brings all your news sources together in a clean, organized interface.",
            problem_title: "The Problem",
            problem_point1: "Information overload from multiple news sources",
            problem_point2: "Constant notifications from various platforms",
            problem_point3: "Being trapped in information bubbles",
            problem_point4: "Difficulty discovering quality content",
            solution_title: "The Solution",
            solution_description: "PmPulse aggregates content from diverse sources and presents it through customizable thematic collections, helping you break out of information bubbles with diverse perspectives.",
            tech_title: "Technology Stack",
            tech_backend: "Backend",
            tech_frontend: "Frontend",
            tech_architecture: "Architecture",
            tech_orleans: "Microsoft Orleans for distributed systems",
            tech_vue: "Vue.js 3 with Composition API",
            tech_tailwind: "Tailwind CSS for styling",
            github_link: "View on GitHub",
            github_description: "Check out the source code and contribute",
            source_code: "Source Code",
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
        about: {
            title: "О PmPulse",
            subtitle: "Современный веб-сервис для агрегации и организации новостей",
            pet_project: "🐶 Это pet-проект",
            overview_title: "Обзор",
            overview_description: "PmPulse помогает вам оставаться в курсе событий без лишнего шума. Вместо подписки на сотни медиа-аккаунтов и постоянных уведомлений, PmPulse объединяет все ваши новостные источники в едином, организованном интерфейсе.",
            problem_title: "Проблема",
            problem_point1: "Перегруженность информацией из множества источников",
            problem_point2: "Постоянные уведомления с разных платформ",
            problem_point3: "Попадание в информационные пузыри",
            problem_point4: "Сложность поиска качественного контента",
            solution_title: "Решение",
            solution_description: "PmPulse агрегирует контент из различных источников и представляет его через настраиваемые тематические коллекции, помогая вам выйти за рамки информационных пузырей благодаря разнообразным перспективам.",
            tech_title: "Технологии",
            tech_backend: "Backend",
            tech_frontend: "Frontend",
            tech_architecture: "Архитектура",
            tech_orleans: "Microsoft Orleans для распределенных систем",
            tech_vue: "Vue.js 3 с Composition API",
            tech_tailwind: "Tailwind CSS для стилизации",
            github_link: "Посмотреть на GitHub",
            github_description: "Изучить исходный код и внести вклад",
            source_code: "Исходный код",
        },
    },
}

export default messages;