'use client';

import { motion } from 'framer-motion';
import { Cookie, Settings, BarChart3, Shield, ToggleLeft, Info } from 'lucide-react';

export default function CookiesPage() {
  return (
    <main className="min-h-screen bg-gradient-to-b from-slate-50 to-white">
      {/* Header */}
      <section className="py-20 bg-gradient-to-br from-slate-900 via-emerald-900 to-slate-900 text-white">
        <div className="container mx-auto px-6 text-center">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <div className="inline-flex items-center gap-2 px-4 py-2 bg-emerald-500/20 rounded-full text-emerald-300 mb-6">
              <Cookie className="w-4 h-4" />
              <span>Cookie Information</span>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Cookie Policy</h1>
            <p className="text-xl text-slate-300 max-w-2xl mx-auto">
              سياسة ملفات تعريف الارتباط - Cookie Policy
            </p>
            <p className="text-sm text-slate-400 mt-4">Last updated: January 2026</p>
          </motion.div>
        </div>
      </section>

      {/* Content */}
      <section className="py-16">
        <div className="container mx-auto px-6 max-w-4xl">
          <div className="prose prose-lg prose-slate max-w-none">
            
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Info className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">1. What Are Cookies?</h2>
              </div>
              <p className="text-slate-600">
                Cookies are small text files that are placed on your device when you visit our website. They help us provide you with a better experience by remembering your preferences and understanding how you use our platform.
              </p>
              <p className="text-slate-600 mt-4" dir="rtl">
                ملفات تعريف الارتباط هي ملفات نصية صغيرة يتم وضعها على جهازك عند زيارة موقعنا. تساعدنا في تقديم تجربة أفضل من خلال تذكر تفضيلاتك وفهم كيفية استخدامك لمنصتنا.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Settings className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">2. Types of Cookies We Use</h2>
              </div>
              
              <div className="space-y-4 mt-4">
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-200">
                  <h3 className="font-semibold text-slate-900 mb-2">Essential Cookies</h3>
                  <p className="text-slate-600 text-sm m-0">
                    Required for the platform to function. These cannot be disabled and include authentication tokens and session management.
                  </p>
                </div>
                
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-200">
                  <h3 className="font-semibold text-slate-900 mb-2">Functional Cookies</h3>
                  <p className="text-slate-600 text-sm m-0">
                    Remember your preferences like language settings, theme choices, and dashboard layouts.
                  </p>
                </div>
                
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-200">
                  <h3 className="font-semibold text-slate-900 mb-2">Analytics Cookies</h3>
                  <p className="text-slate-600 text-sm m-0">
                    Help us understand how visitors interact with our platform to improve user experience.
                  </p>
                </div>
              </div>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <BarChart3 className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">3. Cookie Details</h2>
              </div>
              
              <div className="overflow-x-auto">
                <table className="min-w-full border border-slate-200 rounded-lg">
                  <thead className="bg-slate-100">
                    <tr>
                      <th className="px-4 py-2 text-left text-sm font-semibold text-slate-700">Cookie Name</th>
                      <th className="px-4 py-2 text-left text-sm font-semibold text-slate-700">Purpose</th>
                      <th className="px-4 py-2 text-left text-sm font-semibold text-slate-700">Duration</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-200">
                    <tr>
                      <td className="px-4 py-2 text-sm text-slate-600">auth_token</td>
                      <td className="px-4 py-2 text-sm text-slate-600">Authentication</td>
                      <td className="px-4 py-2 text-sm text-slate-600">Session</td>
                    </tr>
                    <tr>
                      <td className="px-4 py-2 text-sm text-slate-600">locale</td>
                      <td className="px-4 py-2 text-sm text-slate-600">Language preference</td>
                      <td className="px-4 py-2 text-sm text-slate-600">1 year</td>
                    </tr>
                    <tr>
                      <td className="px-4 py-2 text-sm text-slate-600">theme</td>
                      <td className="px-4 py-2 text-sm text-slate-600">UI theme preference</td>
                      <td className="px-4 py-2 text-sm text-slate-600">1 year</td>
                    </tr>
                    <tr>
                      <td className="px-4 py-2 text-sm text-slate-600">_ga</td>
                      <td className="px-4 py-2 text-sm text-slate-600">Analytics</td>
                      <td className="px-4 py-2 text-sm text-slate-600">2 years</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <ToggleLeft className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">4. Managing Cookies</h2>
              </div>
              <p className="text-slate-600">
                You can control and manage cookies through your browser settings. Please note that disabling certain cookies may affect the functionality of our platform.
              </p>
              <ul className="list-disc pl-6 text-slate-600 space-y-2 mt-4">
                <li>Chrome: Settings → Privacy and Security → Cookies</li>
                <li>Firefox: Options → Privacy & Security → Cookies</li>
                <li>Safari: Preferences → Privacy → Cookies</li>
                <li>Edge: Settings → Privacy & Services → Cookies</li>
              </ul>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Shield className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">5. Contact Us</h2>
              </div>
              <p className="text-slate-600">
                If you have questions about our cookie practices, please contact us:
              </p>
              <div className="bg-slate-100 p-4 rounded-lg mt-4">
                <p className="text-slate-700 m-0">
                  <strong>Email:</strong> privacy@shahin-ai.com
                </p>
              </div>
            </motion.div>

          </div>
        </div>
      </section>
    </main>
  );
}
