import React, { useState } from 'react';
import {
  X,
  Send,
  Sparkles,
  BookMarked,
  ShieldCheck,
  Bot,
  User,
  Loader2,
  FileSearch,
} from 'lucide-react';
import { aiApi, AiCitationDto } from '../../api/aiApi';

interface Message {
  id: string;
  sender: 'user' | 'ai';
  text: string;
  citations?: AiCitationDto[];
  model?: string;
  confidence?: number;
  timestamp: string;
}

interface AiChatbotDrawerProps {
  isOpen: boolean;
  onClose: () => void;
}

export const AiChatbotDrawer: React.FC<AiChatbotDrawerProps> = ({ isOpen, onClose }) => {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 'msg-welcome',
      sender: 'ai',
      text: 'Xin chào! Tôi là Trợ lý Học thuật OBE của Đại học Đại Nam. Tôi có thể hỗ trợ bạn tra cứu Chuẩn đầu ra (PLO/PI/CLO), giải thích công thức đo lường hoặc phân tích các kế hoạch cải tiến CQI.',
      timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
    },
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  if (!isOpen) return null;

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
    } catch (err) {
      const errorMsg: Message = {
        id: `msg-err-${Date.now()}`,
        sender: 'ai',
        text: 'Xin lỗi, hiện tại không thể kết nối tới dịch vụ AI. Vui lòng thử lại sau.',
        timestamp: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
      };
      setMessages((prev) => [...prev, errorMsg]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        right: 0,
        bottom: 0,
        width: '450px',
        backgroundColor: 'var(--bg-surface)',
        borderLeft: '1px solid var(--border-strong)',
        boxShadow: '-8px 0 32px rgba(0, 0, 0, 0.4)',
        zIndex: 60,
        display: 'flex',
        flexDirection: 'column',
      }}
      className="animate-slide-right"
    >
      {/* Drawer Header */}
      <div
        style={{
          padding: '1.25rem 1.5rem',
          borderBottom: '1px solid var(--border-medium)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          background: 'var(--primary-gradient-subtle)',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div
            style={{
              width: '36px',
              height: '36px',
              borderRadius: 'var(--radius-sm)',
              background: 'var(--primary-gradient)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: '#fff',
            }}
          >
            <Sparkles size={18} />
          </div>
          <div>
            <h3 style={{ fontSize: '1rem', fontWeight: 700, color: 'var(--text-primary)' }}>
              Trợ Lý AI OBE (RAG)
            </h3>
            <span style={{ fontSize: '0.7rem', color: 'var(--emerald-400)', fontWeight: 600 }}>
              ● Đã kết nối Cơ sở Tri thức
            </span>
          </div>
        </div>

        <button onClick={onClose} className="btn btn-secondary btn-icon">
          <X size={16} />
        </button>
      </div>

      {/* Suggestion Prompts */}
      <div style={{ padding: '0.75rem 1rem', borderBottom: '1px solid var(--border-subtle)', display: 'flex', gap: '0.5rem', overflowX: 'auto' }}>
        {[
          'Tỷ lệ đạt CĐR của ngành CNTT?',
          'Giải thích công thức đo PI?',
          'Kiểm tra độ phủ Bloom CTĐT?',
        ].map((pill, i) => (
          <button
            key={i}
            onClick={() => handleSend(pill)}
            style={{
              padding: '0.3rem 0.6rem',
              borderRadius: 'var(--radius-full)',
              border: '1px solid var(--border-medium)',
              backgroundColor: 'var(--bg-surface-elevated)',
              color: 'var(--text-secondary)',
              fontSize: '0.7rem',
              whiteSpace: 'nowrap',
              cursor: 'pointer',
            }}
          >
            {pill}
          </button>
        ))}
      </div>

      {/* Messages List */}
      <div style={{ flex: 1, padding: '1.25rem', overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        {messages.map((msg) => (
          <div
            key={msg.id}
            style={{
              display: 'flex',
              gap: '0.75rem',
              alignSelf: msg.sender === 'user' ? 'flex-end' : 'flex-start',
              maxWidth: '90%',
            }}
          >
            {msg.sender === 'ai' && (
              <div
                style={{
                  width: '28px',
                  height: '28px',
                  borderRadius: '50%',
                  backgroundColor: 'rgba(99, 102, 241, 0.2)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  color: 'var(--primary-400)',
                  flexShrink: 0,
                }}
              >
                <Bot size={16} />
              </div>
            )}

            <div>
              <div
                style={{
                  padding: '0.75rem 1rem',
                  borderRadius: 'var(--radius-md)',
                  backgroundColor: msg.sender === 'user' ? 'var(--primary-600)' : 'var(--bg-surface-elevated)',
                  color: '#ffffff',
                  fontSize: '0.875rem',
                  lineHeight: '1.5',
                  border: msg.sender === 'ai' ? '1px solid var(--border-medium)' : 'none',
                }}
              >
                {msg.text}

                {/* Academic Citations Cards (FR-AI-02) */}
                {msg.citations && msg.citations.length > 0 && (
                  <div style={{ marginTop: '0.75rem', paddingTop: '0.75rem', borderTop: '1px solid var(--border-subtle)' }}>
                    <p style={{ fontSize: '0.7rem', fontWeight: 700, color: 'var(--primary-400)', marginBottom: '0.4rem', display: 'flex', alignItems: 'center', gap: '0.3rem' }}>
                      <BookMarked size={12} /> Trích dẫn nguồn học thuật:
                    </p>
                    {msg.citations.map((c, idx) => (
                      <div
                        key={idx}
                        style={{
                          backgroundColor: 'var(--bg-surface)',
                          padding: '0.5rem',
                          borderRadius: 'var(--radius-xs)',
                          border: '1px solid var(--border-subtle)',
                          marginBottom: '0.35rem',
                          fontSize: '0.72rem',
                          color: 'var(--text-secondary)',
                        }}
                      >
                        <strong style={{ color: 'var(--text-primary)' }}>{c.title}</strong>
                        {c.pageOrSection && <span> • {c.pageOrSection}</span>}
                        {c.formulaApplied && <div style={{ color: 'var(--cyan-400)', marginTop: '0.2rem', fontFamily: 'var(--font-mono)' }}>Công thức: {c.formulaApplied}</div>}
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginTop: '0.25rem', fontSize: '0.65rem', color: 'var(--text-muted)' }}>
                <span>{msg.timestamp}</span>
                {msg.model && <span>• Mô hình: {msg.model}</span>}
                {msg.confidence && <span>• Độ tin cậy: {(msg.confidence * 100).toFixed(0)}%</span>}
              </div>
            </div>

            {msg.sender === 'user' && (
              <div
                style={{
                  width: '28px',
                  height: '28px',
                  borderRadius: '50%',
                  backgroundColor: 'var(--primary-500)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  color: '#fff',
                  flexShrink: 0,
                }}
              >
                <User size={16} />
              </div>
            )}
          </div>
        ))}

        {isLoading && (
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: 'var(--primary-400)', fontSize: '0.8125rem' }}>
            <Loader2 size={16} className="animate-spin" />
            <span>AI đang tra cứu tài liệu & chuẩn bị câu trả lời...</span>
          </div>
        )}
      </div>

      {/* Input Box */}
      <div style={{ padding: '1rem 1.25rem', borderTop: '1px solid var(--border-medium)', backgroundColor: 'var(--bg-surface-elevated)' }}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleSend();
          }}
          style={{ display: 'flex', gap: '0.5rem' }}
        >
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Hỏi bất kỳ điều gì về chuẩn đầu ra..."
            className="form-input"
            style={{ flex: 1, padding: '0.6rem 0.875rem' }}
          />
          <button type="submit" disabled={isLoading} className="btn btn-primary btn-icon">
            <Send size={16} />
          </button>
        </form>
      </div>
    </div>
  );
};
