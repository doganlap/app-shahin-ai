/**
 * Centralized URL Configuration for shahin-ai-website
 * 
 * All URLs should be loaded from environment variables.
 * Fallback values are provided for development only.
 */

// API Configuration - Standardized on portal.shahin-ai.com (matches ASP.NET backend)
export const API_CONFIG = {
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
    (process.env.NODE_ENV === 'production' 
      ? 'https://portal.shahin-ai.com' 
      : 'http://localhost:8080'),
} as const;

// Portal/App URLs - Standardized on portal.shahin-ai.com
export const PORTAL_CONFIG = {
  APP_URL: process.env.NEXT_PUBLIC_PORTAL_URL || 
    (process.env.NODE_ENV === 'production'
      ? 'https://portal.shahin-ai.com'
      : 'http://localhost:8080'),
  
  LOGIN_URL: (process.env.NEXT_PUBLIC_PORTAL_URL || 
    (process.env.NODE_ENV === 'production'
      ? 'https://portal.shahin-ai.com'
      : 'http://localhost:8080')) + '/Account/Login',
} as const;

/**
 * Validate required environment variables in production
 */
if (process.env.NODE_ENV === 'production') {
  const requiredVars = ['NEXT_PUBLIC_API_URL', 'NEXT_PUBLIC_APP_URL'];
  const missing = requiredVars.filter(v => !process.env[v]);
  
  if (missing.length > 0) {
    console.warn(
      `⚠️ Missing required environment variables: ${missing.join(', ')}\n` +
      `Using fallback values. This should be fixed before deployment.`
    );
  }
}

export default {
  api: API_CONFIG,
  portal: PORTAL_CONFIG,
};
