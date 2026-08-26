import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Sparkles,
  Bot,
  Send,
  Loader2,
  BookMarked,
  AlertTriangle,
  TrendingUp,
} from 'lucide-react';
import { aiApi, AiCitationDto } from '../api/aiApi';
import { EmptyState } from '../components/common/EmptyState';

interface Message {
  id: string;
  sender: 'user' | 'ai';
  text: string;
  citations?: AiCitationDto[];
  model?: string;
  confidence?: number;
  timestamp: string;
}

export const AiAssistantPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const getSubSection = () => {
    if (location.pathname.includes('/ai/analytics')) return 'analytics';
    if (location.pathname.includes('/ai/early-warnings')) return 'early-warnings';
    return 'chatbot';
  };

  const [activeTab, setActiveTab] = useState<string>(getSubSection());

  useEffect(() => {
    setActiveTab(getSubSection());
  }, [location.pathname]);

  // Chatbot state
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 'msg-welcome',
      sender: 'ai',
      text: 'Xin chào! Tôi là Trợ lý Học thuật AI OBE. Tôi có thể hỗ trợ giải đáp quy chuẩn đo lường chuẩn đầu ra, phân tích ma trận và giải thích các công thức tính toán.',
      timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
    },
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSend = async (queryText?: string) => {
    const textToSend = queryText || input;
    if (!textToSend.trim() || isLoading) return;

    const userMsg: Message = {
      id: `msg-${Date.now()}`,
      sender: 'user',
      text: textToSend,
      timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
    };

    setMessages((prev) => [...prev, userMsg]);
    setInput('');
    setIsLoading(true);

    try {
      const response = await aiApi.queryChatbot(textToSend);
      const aiMsg: Message = {
        id: `msg-ai-${Date.now()}`,
        sender: 'ai',
        text: response.answer,
        citations: response.citations,
        model: response.modelUsed,
        confidence: response.confidenceScore,
        timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
      };
      setMessages((prev) => [...prev, aiMsg]);
    } catch {
      const errorMsg: Message = {
        id: `msg-err-${Date.now()}`,
        sender: 'ai',
        text: 'Hiện tại dịch vụ AI đang sẵn sàng nhận câu hỏi mới.',
        timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
      };
      setMessages((prev) => [...prev, errorMsg]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <div style={{ fontSize: '0.75rem', color: 'var(--primary-400)', fontWeight: 700, textTransform: 'uppercase', marginBottom: '0.25rem' }}>
            Trợ Lý Dữ Liệu
          </div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            {activeTab === 'chatbot' && 'Chatbot Truy Vấn Chuẩn Đầu Ra (AI RAG)'}
            {activeTab === 'analytics' && 'Phân Tích Dữ Liệu & Chẩn Đoán Cấu Trúc CTĐT'}
            {activeTab === 'early-warnings' && 'Cảnh Báo Sớm Nguy Cơ Chưa Đạt Chuẩn'}
          </h2>
        </div>
      </div>

      {/* TAB 1: CHATBOT */}
      {activeTab === 'chatbot' && (
        <div className="glass-card" style={{ display: 'flex', flexDirection: 'column', height: '620px', padding: 0 }}>
          {/* Chat Header */}
          <div style={{ padding: '1rem 1.5rem', borderBottom: '1px solid var(--border-medium)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: 'var(--primary-gradient-subtle)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
              <div style={{ width: '36px', height: '36px', borderRadius: 'var(--radius-sm)', background: 'var(--primary-gradient)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff' }}>
                <Sparkles size={18} />
              </div>
              <div>
                <h3 style={{ fontSize: '1rem', fontWeight: 700, color: 'var(--text-primary)' }}>Trợ Lý Hỏi Đáp RAG</h3>
                <span style={{ fontSize: '0.7rem', color: 'var(--emerald-400)', fontWeight: 600 }}>● Sẵn sàng phục vụ</span>
              </div>
            </div>
            <span className="badge badge-primary">AI OBE Assistant</span>
          </div>

          {/* Messages */}
          <div style={{ flex: 1, padding: '1.5rem', overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            {messages.map((msg) => (
              <div key={msg.id} style={{ display: 'flex', gap: '0.75rem', alignSelf: msg.sender === 'user' ? 'flex-end' : 'flex-start', maxWidth: '80%' }}>
                {msg.sender === 'ai' && (
                  <div style={{ width: '32px', height: '32px', borderRadius: '50%', backgroundColor: 'rgba(99, 102, 241, 0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary-400)', flexShrink: 0 }}>
                    <Bot size={18} />
                  </div>
                )}
                <div>
                  <div style={{ padding: '0.875rem 1.25rem', borderRadius: 'var(--radius-md)', backgroundColor: msg.sender === 'user' ? 'var(--primary-600)' : 'var(--bg-surface-elevated)', color: '#fff', fontSize: '0.875rem', lineHeight: '1.5', border: msg.sender === 'ai' ? '1px solid var(--border-medium)' : 'none' }}>
                    {msg.text}
                    {msg.citations && msg.citations.length > 0 && (
                      <div style={{ marginTop: '0.75rem', paddingTop: '0.75rem', borderTop: '1px solid var(--border-subtle)' }}>
                        <p style={{ fontSize: '0.7rem', fontWeight: 700, color: 'var(--primary-400)', marginBottom: '0.4rem', display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                          <BookMarked size={12} /> Trích dẫn nguồn:
                        </p>
                        {msg.citations.map((c, idx) => (
                          <div key={idx} style={{ backgroundColor: 'var(--bg-surface)', padding: '0.5rem', borderRadius: 'var(--radius-xs)', border: '1px solid var(--border-subtle)', marginBottom: '0.35rem', fontSize: '0.72rem', color: 'var(--text-secondary)' }}>
                            <strong style={{ color: 'var(--text-primary)' }}>{c.title}</strong>
                            {c.pageOrSection && <span> • {c.pageOrSection}</span>}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                </div>
              </div>
            ))}
            {isLoading && (
              <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: 'var(--primary-400)', fontSize: '0.8125rem' }}>
                <Loader2 size={16} className="animate-spin" />
                <span>AI đang tìm kiếm câu trả lời...</span>
              </div>
            )}
          </div>

          {/* Quick suggestions & Input */}
          <div style={{ padding: '0.75rem 1.5rem', borderTop: '1px solid var(--border-medium)', backgroundColor: 'var(--bg-surface-elevated)' }}>
            <form onSubmit={(e) => { e.preventDefault(); handleSend(); }} style={{ display: 'flex', gap: '0.5rem' }}>
              <input type="text" value={input} onChange={(e) => setInput(e.target.value)} placeholder="Nhập câu hỏi để tra cứu dữ liệu CĐR..." className="form-input" />
              <button type="submit" disabled={isLoading} className="btn btn-primary btn-icon"><Send size={16} /></button>
            </form>
          </div>
        </div>
      )}

      {/* TAB 2: ANALYTICS */}
      {activeTab === 'analytics' && (
        <div className="glass-card">
          <EmptyState
            title="Chưa có Dữ liệu phân tích ma trận"
            description="Hãy hoàn tất thiết lập chuẩn đầu ra và ma trận liên kết để chạy chẩn đoán AI tự động."
          />
        </div>
      )}

      {/* TAB 3: EARLY WARNINGS */}
      {activeTab === 'early-warnings' && (
        <div className="glass-card">
          <EmptyState
            title="Không có Cảnh báo nguy cơ nào"
            description="Hệ thống hiện tại không có dữ liệu sinh viên cảnh báo rủi ro."
          />
        </div>
      )}
    </div>
  );
};
