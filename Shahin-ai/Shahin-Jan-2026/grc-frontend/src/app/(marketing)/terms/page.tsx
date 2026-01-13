'use client';

import { motion } from 'framer-motion';
import { FileText, Scale, AlertTriangle, CheckCircle, XCircle, Gavel } from 'lucide-react';

export default function TermsPage() {
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
              <FileText className="w-4 h-4" />
              <span>Legal Agreement</span>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold mb-4">Terms & Conditions</h1>
            <p className="text-xl text-slate-300 max-w-2xl mx-auto">
              الشروط والأحكام - Terms of Service
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
                <Scale className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">1. Acceptance of Terms</h2>
              </div>
              <p className="text-slate-600">
                By accessing and using the Shahin AI GRC platform (&quot;Service&quot;), you accept and agree to be bound by these Terms and Conditions. If you do not agree to these terms, please do not use our Service.
              </p>
              <p className="text-slate-600 mt-4" dir="rtl">
                من خلال الوصول إلى منصة شاهين للذكاء الاصطناعي واستخدامها، فإنك تقبل وتوافق على الالتزام بهذه الشروط والأحكام.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <CheckCircle className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">2. Use of Service</h2>
              </div>
              <p className="text-slate-600">You agree to use the Service only for lawful purposes and in accordance with these Terms. You agree not to:</p>
              <ul className="list-disc pl-6 text-slate-600 space-y-2 mt-2">
                <li>Use the Service in violation of any applicable laws or regulations</li>
                <li>Attempt to gain unauthorized access to any portion of the Service</li>
                <li>Interfere with or disrupt the Service or servers</li>
                <li>Upload malicious code or content</li>
                <li>Share your account credentials with unauthorized parties</li>
              </ul>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <FileText className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">3. Intellectual Property</h2>
              </div>
              <p className="text-slate-600">
                The Service and its original content, features, and functionality are owned by Shahin AI and are protected by international copyright, trademark, patent, trade secret, and other intellectual property laws.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <AlertTriangle className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">4. Limitation of Liability</h2>
              </div>
              <p className="text-slate-600">
                To the maximum extent permitted by law, Shahin AI shall not be liable for any indirect, incidental, special, consequential, or punitive damages, or any loss of profits or revenues, whether incurred directly or indirectly.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <XCircle className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">5. Termination</h2>
              </div>
              <p className="text-slate-600">
                We may terminate or suspend your access to the Service immediately, without prior notice, for conduct that we believe violates these Terms or is harmful to other users, us, or third parties, or for any other reason.
              </p>
            </motion.div>

            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="mb-12"
            >
              <div className="flex items-center gap-3 mb-4">
                <Gavel className="w-6 h-6 text-emerald-600" />
                <h2 className="text-2xl font-bold text-slate-900 m-0">6. Governing Law</h2>
              </div>
              <p className="text-slate-600">
                These Terms shall be governed by and construed in accordance with the laws of the Kingdom of Saudi Arabia, without regard to its conflict of law provisions.
              </p>
              <div className="bg-slate-100 p-4 rounded-lg mt-4">
                <p className="text-slate-700 m-0">
                  <strong>Contact:</strong> legal@shahin-ai.com
                </p>
              </div>
            </motion.div>

          </div>
        </div>
      </section>
    </main>
  );
}
