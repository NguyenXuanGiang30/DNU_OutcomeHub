import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
  Sparkles,
  Bot,
  Send,
  Loader2,
  BookMarked,
  AlertTriangle,
  FileSearch,
  CheckCircle,
  TrendingUp,
} from 'lucide-react';
import { aiApi, AiCitationDto } from '../api/aiApi';

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

  const handleTabClick = (key: string) => {
    setActiveTab(key);
    navigate(`/ai/${key}`);
  };

  // Chatbot state
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 'msg-welcome',
      sender: 'ai',
      text: 'Xin chào! Tôi là Trợ lý Học thuật AI OBE của Đại học Đại Nam. Tôi có thể giúp bạn tra cứu ma trận CĐR, giải thích chi tiết công thức tính toán điểm PI/PLO, phát hiện mâu thuẫn trong đề cương BM13 hoặc dự báo sớm nhóm sinh viên có nguy cơ chưa đạt chuẩn.',
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
        text: 'Xin lỗi, hiện tại không thể kết nối tới dịch vụ AI. Vui lòng thử lại sau.',
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
            {activeTab === 'chatbot' && 'Chatbot Truy Vấn Chuẩn Đầu Ra (AI RAG Assistant)'}
            {activeTab === 'analytics' && 'Phân Tích Dữ Liệu & Chẩn Đoán Cấu Trúc CTĐT'}
            {activeTab === 'early-warnings' && 'Cảnh Báo Sớm Nguy Cơ Sinh Viên Chưa Đạt Chuẩn'}
          </h2>
        </div>
      </div>

      {/* TAB 1: CHATBOT */}
      {activeTab === 'chatbot' && (
        <div className="glass-card" style={{ display: 'flex', flexDirection: 'column', height: '650px', padding: 0 }}>
          {/* Chat Header */}
          <div style={{ padding: '1rem 1.5rem', borderBottom: '1px solid var(--border-medium)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: 'var(--primary-gradient-subtle)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
              <div style={{ width: '36px', height: '36px', borderRadius: 'var(--radius-sm)', background: 'var(--primary-gradient)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: '#fff' }}>
                <Sparkles size={18} />
              </div>
              <div>
                <h3 style={{ fontSize: '1rem', fontWeight: 700, color: 'var(--text-primary)' }}>Trợ Lý Hỏi Đáp Thông Minh (RAG)</h3>
                <span style={{ fontSize: '0.7rem', color: 'var(--emerald-400)', fontWeight: 600 }}>● Cơ sở tri thức CĐR K15 - K18</span>
              </div>
            </div>
            <span className="badge badge-primary">Mô hình: Gemini 1.5 Pro</span>
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
                          <BookMarked size={12} /> Trích dẫn nguồn học thuật:
                        </p>
                        {msg.citations.map((c, idx) => (
                          <div key={idx} style={{ backgroundColor: 'var(--bg-surface)', padding: '0.5rem', borderRadius: 'var(--radius-xs)', border: '1px solid var(--border-subtle)', marginBottom: '0.35rem', fontSize: '0.72rem', color: 'var(--text-secondary)' }}>
                            <strong style={{ color: 'var(--text-primary)' }}>{c.title}</strong>
                            {c.pageOrSection && <span> • {c.pageOrSection}</span>}
                            {c.formulaApplied && <div style={{ color: 'var(--cyan-400)', marginTop: '0.2rem', fontFamily: 'var(--font-mono)' }}>Công thức: {c.formulaApplied}</div>}
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
                <span>AI đang phân tích dữ liệu & chuẩn bị câu trả lời...</span>
              </div>
            )}
          </div>

          {/* Quick suggestions & Input */}
          <div style={{ padding: '0.75rem 1.5rem', borderTop: '1px solid var(--border-medium)', backgroundColor: 'var(--bg-surface-elevated)' }}>
            <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.5rem', overflowX: 'auto' }}>
              {['Khóa K17 có bao nhiêu PLO đạt chuẩn?', 'Công thức tính tỷ lệ đạt PI 3.1?', 'Học phần nào đang đảm nhận đo PI 5.1?'].map((q, i) => (
                <button key={i} onClick={() => handleSend(q)} className="btn btn-sm btn-secondary" style={{ whiteSpace: 'nowrap' }}>{q}</button>
              ))}
            </div>
            <form onSubmit={(e) => { e.preventDefault(); handleSend(); }} style={{ display: 'flex', gap: '0.5rem' }}>
              <input type="text" value={input} onChange={(e) => setInput(e.target.value)} placeholder="Nhập câu hỏi cần tra cứu..." className="form-input" />
              <button type="submit" disabled={isLoading} className="btn btn-primary btn-icon"><Send size={16} /></button>
            </form>
          </div>
        </div>
      )}

      {/* TAB 2: ANALYTICS */}
      {activeTab === 'analytics' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Chẩn Đoán Tự Động Mâu Thuẫn Cấu Trúc CTĐT (AI Matrix Diagnostics)</h3>
              <p className="glass-card-subtitle">Phát hiện học phần thiếu đo trực tiếp (A), thừa mức độ hoặc lệch bậc Bloom</p>
            </div>
            <button className="btn btn-sm btn-primary">Chạy Chẩn Đoán Toàn Diện</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--emerald-500)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.35rem' }}>
                <strong style={{ color: 'var(--emerald-400)' }}>✓ Độ Phủ Trọng Số A: ĐẠT CHUẨN (100%)</strong>
                <span className="badge badge-success">KHÔNG CÓ LỖ HỔNG</span>
              </div>
              <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>Mọi chỉ số PI trong CTĐT KTPM v2023 đều có ít nhất 1 học phần đo trực tiếp A với tổng tỷ trọng đúng 100%.</p>
            </div>

            <div style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--cyan-500)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.35rem' }}>
                <strong style={{ color: 'var(--cyan-400)' }}>✓ Tính Liên Tục Của Bậc Năng Lực Bloom</strong>
                <span className="badge badge-cyan">TIẾN TRÌNH HỢP LÝ</span>
              </div>
              <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>Tiến trình học phần tăng dần đều từ Mức 1-2 (Năm 1) ➔ Mức 3 (Năm 2) ➔ Mức 4-5 (Năm 3) ➔ Mức 6 (Khóa luận tốt nghiệp).</p>
            </div>
          </div>
        </div>
      )}

      {/* TAB 3: EARLY WARNINGS */}
      {activeTab === 'early-warnings' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Mô Hình AI Dự Báo Sinh Viên Nguy Cơ Chưa Đạt Chuẩn (Early Warning)</h3>
              <p className="glass-card-subtitle">Phát hiện sớm dựa trên kết quả bài tập quá trình A1 và điểm danh LMS</p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Sinh Viên</th>
                  <th>Họ Và Tên</th>
                  <th>Lớp Sinh Hoạt</th>
                  <th>CĐR Có Nguy Cơ</th>
                  <th>Mức Độ Rủi Ro</th>
                  <th>Hành Động Khuyến Nghị Của AI</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>20230003</code></td>
                  <td>Lê Hoàng Cường</td>
                  <td>17IT01</td>
                  <td><strong>PI 5.1 (Unit Testing)</strong></td>
                  <td><span className="badge badge-danger">RỦI RO CAO (85%)</span></td>
                  <td>Gửi thông báo phụ đạo chuyên đề Unit Test bổ sung 2 buổi</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
