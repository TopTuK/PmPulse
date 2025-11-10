<template>
    <footer class="h-16 bg-gray-700 text-white flex items-center justify-center gap-4">
        <p>
            {{ $t('common.author_name') }} &copy; {{ $t('common.created_date') }}
        </p>
        <span class="text-gray-400">|</span>
        <router-link 
            :to="{ name: 'Digest' }" 
            class="text-gray-300 hover:text-white transition-colors duration-200 underline"
        >
            {{ $t('weekly_digest.title') }}
        </router-link>
        <span class="text-gray-400">|</span>
        <router-link 
            to="/about" 
            class="text-gray-300 hover:text-white transition-colors duration-200 underline"
        >
            {{ $t('about.title') }}
        </router-link>
    </footer>
</template>