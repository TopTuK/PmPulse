<script setup>
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const githubUrl = 'https://github.com/TopTuK/PmPulse'
</script>

<template>
    <div class="flex flex-col w-full min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-purple-50 py-4 sm:py-8 overflow-x-hidden">
        <div class="flex flex-col w-full max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
            
            <!-- Hero Section -->
            <div class="flex flex-col items-center text-center mb-8 sm:mb-12">
                <!-- Pet Project Badge -->
                <div class="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-gradient-to-r from-amber-100 to-orange-100 border border-amber-200 shadow-lg mb-4 animate-pulse">
                    <span class="text-2xl">🐶</span>
                    <span class="text-sm sm:text-base font-bold text-amber-800">{{ $t('about.pet_project') }}</span>
                </div>

                <!-- Title -->
                <h1 class="text-4xl sm:text-5xl lg:text-6xl font-bold bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent mb-4 drop-shadow-lg">
                    {{ $t('about.title') }}
                </h1>
                
                <!-- Subtitle -->
                <p class="text-lg sm:text-xl text-gray-600 max-w-3xl leading-relaxed mb-8">
                    {{ $t('about.subtitle') }}
                </p>

                <!-- GitHub Link -->
                <a 
                    :href="githubUrl" 
                    target="_blank" 
                    rel="noopener noreferrer"
                    class="inline-flex items-center gap-3 px-6 py-3 sm:px-8 sm:py-4 rounded-xl bg-gradient-to-r from-gray-800 to-gray-900 hover:from-gray-900 hover:to-black text-white font-semibold shadow-xl hover:shadow-2xl transition-all duration-300 transform hover:scale-105"
                >
                    <svg class="w-5 h-5 sm:w-6 sm:h-6" fill="currentColor" viewBox="0 0 24 24">
                        <path fill-rule="evenodd" d="M12 2C6.477 2 2 6.477 2 12c0 4.42 2.865 8.17 6.839 9.49.5.092.682-.217.682-.482 0-.237-.008-.866-.013-1.7-2.782.603-3.369-1.34-3.369-1.34-.454-1.156-1.11-1.463-1.11-1.463-.908-.62.069-.608.069-.608 1.003.07 1.531 1.03 1.531 1.03.892 1.529 2.341 1.087 2.91.831.092-.646.35-1.086.636-1.336-2.22-.253-4.555-1.11-4.555-4.943 0-1.091.39-1.984 1.029-2.683-.103-.253-.446-1.27.098-2.647 0 0 .84-.269 2.75 1.025A9.578 9.578 0 0112 6.836c.85.004 1.705.114 2.504.336 1.909-1.294 2.747-1.025 2.747-1.025.546 1.377.203 2.394.1 2.647.64.699 1.028 1.592 1.028 2.683 0 3.842-2.339 4.687-4.566 4.935.359.309.678.919.678 1.852 0 1.336-.012 2.415-.012 2.743 0 .267.18.578.688.48C19.138 20.167 22 16.418 22 12c0-5.523-4.477-10-10-10z" clip-rule="evenodd"/>
                    </svg>
                    <span class="text-base sm:text-lg">{{ $t('about.github_link') }}</span>
                </a>
            </div>

            <!-- Content Grid -->
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 sm:gap-8 mb-8">
                
                <!-- Overview Card -->
                <div class="bg-white rounded-2xl shadow-xl p-6 sm:p-8 border border-gray-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center gap-3 mb-4">
                        <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600">
                            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <h2 class="text-2xl font-bold text-gray-800">{{ $t('about.overview_title') }}</h2>
                    </div>
                    <p class="text-gray-600 leading-relaxed">
                        {{ $t('about.overview_description') }}
                    </p>
                </div>

                <!-- Problem Card -->
                <div class="bg-gradient-to-br from-red-50 to-orange-50 rounded-2xl shadow-xl p-6 sm:p-8 border border-red-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center gap-3 mb-4">
                        <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-red-500 to-red-600">
                            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                            </svg>
                        </div>
                        <h2 class="text-2xl font-bold text-gray-800">{{ $t('about.problem_title') }}</h2>
                    </div>
                    <ul class="space-y-2">
                        <li class="flex items-start gap-2">
                            <svg class="w-5 h-5 text-red-500 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                            </svg>
                            <span class="text-gray-700">{{ $t('about.problem_point1') }}</span>
                        </li>
                        <li class="flex items-start gap-2">
                            <svg class="w-5 h-5 text-red-500 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                            </svg>
                            <span class="text-gray-700">{{ $t('about.problem_point2') }}</span>
                        </li>
                        <li class="flex items-start gap-2">
                            <svg class="w-5 h-5 text-red-500 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                            </svg>
                            <span class="text-gray-700">{{ $t('about.problem_point3') }}</span>
                        </li>
                        <li class="flex items-start gap-2">
                            <svg class="w-5 h-5 text-red-500 mt-0.5 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                            </svg>
                            <span class="text-gray-700">{{ $t('about.problem_point4') }}</span>
                        </li>
                    </ul>
                </div>

                <!-- Solution Card -->
                <div class="bg-gradient-to-br from-green-50 to-teal-50 rounded-2xl shadow-xl p-6 sm:p-8 border border-green-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center gap-3 mb-4">
                        <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-green-500 to-green-600">
                            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <h2 class="text-2xl font-bold text-gray-800">{{ $t('about.solution_title') }}</h2>
                    </div>
                    <p class="text-gray-600 leading-relaxed">
                        {{ $t('about.solution_description') }}
                    </p>
                </div>

                <!-- Technology Card -->
                <div class="bg-gradient-to-br from-purple-50 to-pink-50 rounded-2xl shadow-xl p-6 sm:p-8 border border-purple-100 hover:shadow-2xl transition-all duration-300">
                    <div class="flex items-center gap-3 mb-4">
                        <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600">
                            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
                            </svg>
                        </div>
                        <h2 class="text-2xl font-bold text-gray-800">{{ $t('about.tech_title') }}</h2>
                    </div>
                    <div class="space-y-4">
                        <div>
                            <h3 class="font-bold text-purple-700 mb-2">{{ $t('about.tech_backend') }}</h3>
                            <p class="text-gray-600 text-sm">{{ $t('about.tech_orleans') }}</p>
                        </div>
                        <div>
                            <h3 class="font-bold text-purple-700 mb-2">{{ $t('about.tech_frontend') }}</h3>
                            <p class="text-gray-600 text-sm">{{ $t('about.tech_vue') }}</p>
                            <p class="text-gray-600 text-sm">{{ $t('about.tech_tailwind') }}</p>
                        </div>
                    </div>
                </div>

            </div>

            <!-- Architecture Diagram Card -->
            <div class="bg-white rounded-2xl shadow-xl p-6 sm:p-8 border border-gray-100 mb-8">
                <div class="flex items-center gap-3 mb-6">
                    <div class="flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-br from-indigo-500 to-indigo-600">
                        <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
                        </svg>
                    </div>
                    <h2 class="text-2xl font-bold text-gray-800">{{ $t('about.tech_architecture') }}</h2>
                </div>
                
                <div class="relative bg-gradient-to-br from-indigo-50 to-blue-50 rounded-xl p-6 sm:p-8 border border-indigo-100 overflow-hidden">
                    <!-- Decorative circles -->
                    <div class="absolute top-0 right-0 w-32 h-32 bg-blue-200 rounded-full blur-3xl opacity-20"></div>
                    <div class="absolute bottom-0 left-0 w-40 h-40 bg-purple-200 rounded-full blur-3xl opacity-20"></div>
                    
                    <div class="relative">
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
                            <div class="flex flex-col items-center p-4 bg-white rounded-lg shadow-md border border-gray-200">
                                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-green-400 to-green-600 flex items-center justify-center mb-3">
                                    <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                                        <path d="M2 6a2 2 0 012-2h6a2 2 0 012 2v8a2 2 0 01-2 2H4a2 2 0 01-2-2V6zM14.553 7.106A1 1 0 0014 8v4a1 1 0 00.553.894l2 1A1 1 0 0018 13V7a1 1 0 00-1.447-.894l-2 1z" />
                                    </svg>
                                </div>
                                <h4 class="font-bold text-gray-800 mb-1">Vue.js App</h4>
                                <p class="text-xs text-gray-600 text-center">Frontend</p>
                            </div>
                            
                            <div class="flex flex-col items-center p-4 bg-white rounded-lg shadow-md border border-gray-200">
                                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-blue-400 to-blue-600 flex items-center justify-center mb-3">
                                    <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                                        <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z" />
                                    </svg>
                                </div>
                                <h4 class="font-bold text-gray-800 mb-1">Web API</h4>
                                <p class="text-xs text-gray-600 text-center">RESTful API</p>
                            </div>
                            
                            <div class="flex flex-col items-center p-4 bg-white rounded-lg shadow-md border border-gray-200">
                                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-purple-400 to-purple-600 flex items-center justify-center mb-3">
                                    <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                                        <path fill-rule="evenodd" d="M11.3 1.046A1 1 0 0112 2v5h4a1 1 0 01.82 1.573l-7 10A1 1 0 018 18v-5H4a1 1 0 01-.82-1.573l7-10a1 1 0 011.12-.38z" clip-rule="evenodd" />
                                    </svg>
                                </div>
                                <h4 class="font-bold text-gray-800 mb-1">Orleans Silo</h4>
                                <p class="text-xs text-gray-600 text-center">Distributed Grains</p>
                            </div>
                        </div>
                        
                        <!-- Arrows -->
                        <div class="hidden md:flex items-center justify-center gap-2 mt-4">
                            <svg class="w-4 h-4 text-indigo-500" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
                            </svg>
                            <svg class="w-4 h-4 text-indigo-500" fill="currentColor" viewBox="0 0 20 20">
                                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
                            </svg>
                        </div>
                    </div>
                </div>
            </div>

            <!-- GitHub Section -->
            <div class="bg-gradient-to-br from-gray-800 to-gray-900 rounded-2xl shadow-2xl p-8 sm:p-12 text-center border border-gray-700">
                <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 rounded-full bg-white/10 backdrop-blur-sm border border-white/20">
                    <svg class="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 24 24">
                        <path fill-rule="evenodd" d="M12 2C6.477 2 2 6.477 2 12c0 4.42 2.865 8.17 6.839 9.49.5.092.682-.217.682-.482 0-.237-.008-.866-.013-1.7-2.782.603-3.369-1.34-3.369-1.34-.454-1.156-1.11-1.463-1.11-1.463-.908-.62.069-.608.069-.608 1.003.07 1.531 1.03 1.531 1.03.892 1.529 2.341 1.087 2.91.831.092-.646.35-1.086.636-1.336-2.22-.253-4.555-1.11-4.555-4.943 0-1.091.39-1.984 1.029-2.683-.103-.253-.446-1.27.098-2.647 0 0 .84-.269 2.75 1.025A9.578 9.578 0 0112 6.836c.85.004 1.705.114 2.504.336 1.909-1.294 2.747-1.025 2.747-1.025.546 1.377.203 2.394.1 2.647.64.699 1.028 1.592 1.028 2.683 0 3.842-2.339 4.687-4.566 4.935.359.309.678.919.678 1.852 0 1.336-.012 2.415-.012 2.743 0 .267.18.578.688.48C19.138 20.167 22 16.418 22 12c0-5.523-4.477-10-10-10z" clip-rule="evenodd"/>
                    </svg>
                </div>
                
                <h3 class="text-2xl sm:text-3xl font-bold text-white mb-3">
                    {{ $t('about.source_code') }}
                </h3>
                <p class="text-gray-300 mb-6 max-w-2xl mx-auto">
                    {{ $t('about.github_description') }}
                </p>
                
                <a 
                    :href="githubUrl" 
                    target="_blank" 
                    rel="noopener noreferrer"
                    class="inline-flex items-center gap-3 px-8 py-4 rounded-xl bg-white hover:bg-gray-100 text-gray-900 font-bold shadow-xl hover:shadow-2xl transition-all duration-300 transform hover:scale-105"
                >
                    <svg class="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                        <path fill-rule="evenodd" d="M12 2C6.477 2 2 6.477 2 12c0 4.42 2.865 8.17 6.839 9.49.5.092.682-.217.682-.482 0-.237-.008-.866-.013-1.7-2.782.603-3.369-1.34-3.369-1.34-.454-1.156-1.11-1.463-1.11-1.463-.908-.62.069-.608.069-.608 1.003.07 1.531 1.03 1.531 1.03.892 1.529 2.341 1.087 2.91.831.092-.646.35-1.086.636-1.336-2.22-.253-4.555-1.11-4.555-4.943 0-1.091.39-1.984 1.029-2.683-.103-.253-.446-1.27.098-2.647 0 0 .84-.269 2.75 1.025A9.578 9.578 0 0112 6.836c.85.004 1.705.114 2.504.336 1.909-1.294 2.747-1.025 2.747-1.025.546 1.377.203 2.394.1 2.647.64.699 1.028 1.592 1.028 2.683 0 3.842-2.339 4.687-4.566 4.935.359.309.678.919.678 1.852 0 1.336-.012 2.415-.012 2.743 0 .267.18.578.688.48C19.138 20.167 22 16.418 22 12c0-5.523-4.477-10-10-10z" clip-rule="evenodd"/>
                    </svg>
                    <span>GitHub</span>
                </a>
            </div>

            <!-- Footer Tagline -->
            <div class="text-center mt-8 mb-4">
                <p class="text-lg sm:text-xl font-bold text-gray-700 italic">
                    Stay informed. Stay curious. Break the bubble.
                </p>
            </div>

        </div>
    </div>
</template>

<style scoped>
/* Smooth animations */
@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* Apply animation to cards */
div > div > div[class*="bg-white"],
div > div > div[class*="from-red-50"],
div > div > div[class*="from-green-50"],
div > div > div[class*="from-purple-50"] {
    animation: fadeInUp 0.6s ease-out;
}

/* Ensure no horizontal overflow */
:deep(*) {
    max-width: 100%;
    box-sizing: border-box;
}

/* Prevent any child components from causing horizontal scroll */
:deep([class*="grid"]),
:deep([class*="flex"]) {
    max-width: 100%;
    overflow-x: hidden;
}
</style>