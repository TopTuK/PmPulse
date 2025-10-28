# PmPulse Frontend

Vue.js frontend application for PmPulse - a web service for aggregating content from multiple RSS feeds and Telegram channels.

## Tech Stack

- **Vue.js 3**: Progressive JavaScript framework with Composition API
- **Vite 7**: Fast build tool and development server
- **Pinia**: State management
- **Vue Router**: Client-side routing
- **Vue I18n**: Internationalization support
- **Vuestic UI**: Vue 3 component library
- **Tailwind CSS 4**: Utility-first CSS framework
- **Axios**: HTTP client for API communication

## Prerequisites

- **Node.js**: ^20.19.0 || >=22.12.0
- **npm** or **yarn**

## Setup

Install dependencies:

```sh
npm install
```

## Development

Start the development server with hot reload:

```sh
npm run dev
```

The frontend will be available at `http://localhost:5173`

**Note**: Make sure the backend services are running:
- Web API on `http://localhost:8080` (or configure via environment variables)
- Orleans Silo for feed processing

## Build

Build for production:

```sh
npm run build
```

The production build will be in the `dist/` directory.

## Preview Production Build

Preview the production build locally:

```sh
npm run preview
```

## Linting

Lint and fix code:

```sh
npm run lint
```

## Project Structure

```
webapp/
├── src/
│   ├── components/       # Reusable Vue components
│   │   ├── BlockFeedNews.vue
│   │   └── FeedBlockSelector.vue
│   ├── views/            # Page views
│   │   ├── MainView.vue
│   │   └── FeedView.vue
│   ├── layout/           # Layout components
│   │   ├── MainLayout.vue
│   │   ├── HeaderLayout.vue
│   │   └── FooterLayout.vue
│   ├── services/         # API clients
│   │   ├── apiService.js
│   │   ├── feedBlockService.js
│   │   └── feedService.js
│   ├── stores/           # Pinia stores
│   │   └── feedBlockStore.js
│   ├── router/           # Vue Router configuration
│   │   └── index.js
│   ├── locs/             # Internationalization
│   │   └── messages.js
│   ├── assets/           # Static assets
│   ├── main.js           # Application entry point
│   ├── main.css          # Global styles
│   └── utils.js          # Utility functions
├── public/               # Static files
├── index.html            # HTML template
├── vite.config.js        # Vite configuration
├── package.json          # Dependencies and scripts
└── eslint.config.js      # ESLint configuration
```

## Environment Configuration

Create a `.env` file to configure the API endpoint:

```env
VITE_API_BASE_URL=http://localhost:8080
```

## Docker Deployment

The frontend is containerized using Nginx. See the root [README.md](../README.md) and [DOCKER.md](../DOCKER.md) for deployment instructions.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur)

## Features

- **Multi-source aggregation**: View content from RSS feeds and Telegram channels
- **Feed blocks**: Organize feeds by theme or topic
- **Real-time updates**: Automatic feed synchronization
- **Responsive design**: Works on desktop and mobile
- **Internationalization**: i18n support for multiple languages
- **Dark theme**: Modern UI with dark mode support
