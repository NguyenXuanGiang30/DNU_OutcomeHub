import React, { useState } from 'react';
import {
  BookOpen,
  FileSpreadsheet,
  Layers,
  FileCheck,
  FolderArchive,
  Download,
  CheckCircle2,
  ShieldCheck,
} from 'lucide-react';

export const SyllabusPortfolioPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'bm13' | 'blueprint' | 'table831' | 'table832' | 'vault'>('table832');

  return (
    <div className="animate-fade-in">
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Đề Cương, Ma Trận Đề Thi & Bảng 8.3 (Mục 8.3)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý Đề cương BM13, Exam Blueprint, Bảng 8.3.1 (Truy vết), Bảng 8.3.2 (Đo trực tiếp 100%) và Kho tài liệu học thuật.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Gói Portfolio (.ZIP)</span>
          </button>
          <button className="btn btn-primary">
            <CheckCircle2 size={16} />
            <span>Ban Hành Đề Cương (Checklist)</span>
          </button>
        </div>
      </div>

      {/* Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem', overflowX: 'auto' }}>
        {[
          { key: 'table832', label: 'Bảng 8.3.2: Tỷ Trọng Đo Trực Tiếp PI (100%)', icon: Layers, badge: 'Cốt Lõi' },
          { key: 'table831', label: 'Bảng 8.3.1: Ma Trận Truy Vết Toàn Diện', icon: FileSpreadsheet },
          { key: 'blueprint', label: 'Ma Trận Đề Thi (Exam Blueprint)', icon: FileCheck },
          { key: 'bm13', label: 'Đề Cương Chi Tiết (Mẫu BM13)', icon: BookOpen },
          { key: 'vault', label: 'Kho Tài Liệu & Minh Chứng (Vault)', icon: FolderArchive },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key as any)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
            {tab.badge && <span className="badge badge-bloom badge-cyan">{tab.badge}</span>}
          </button>
        ))}
      </div>

      {/* TAB: BẢNG 8.3.2 - PHÂN BỔ TỶ TRỌNG TRỰC TIẾP PI (FR-PRT-05, 18, 19) */}
      {activeTab === 'table832' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <Layers size={20} className="text-emerald-400" />
                Bảng 8.3.2: Khai Báo Tỷ Trọng Trực Tiếp Từng Tiêu Chí Trong PI (Tổng Đúng 100%)
              </h3>
              <p className="glass-card-subtitle">
                Học phần: <strong>IT4101 - Lập trình .NET nâng cao (3 Tín chỉ)</strong> | Đảm nhận đo trực tiếp: <strong>PI 3.1, PI 5.1</strong>
              </p>
            </div>
            <span className="badge badge-success">Kiểm tra hợp lệ 100%</span>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chỉ Số PI Giao Đo (A)</th>
                  <th>Bài Đánh Giá</th>
                  <th>Mã Tiêu Chí Rubric</th>
                  <th>Nội Dung Tiêu Chí Đánh Giá</th>
                  <th>CLO</th>
                  <th>Điểm Tối Đa</th>
                  <th style={{ textAlign: 'center' }}>Tỷ Trọng Trong PI (%)</th>
                  <th style={{ textAlign: 'center' }}>Trạng Thái Tổng</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td rowSpan={2} style={{ fontWeight: 800, color: 'var(--primary-400)', verticalAlign: 'middle', borderRight: '1px solid var(--border-subtle)' }}>
                    PI 3.1
                    <div style={{ fontSize: '0.72rem', color: 'var(--text-secondary)', fontWeight: 400 }}>Hiện thực hóa giải pháp phần mềm</div>
                  </td>
                  <td><span className="badge badge-secondary">A2: Bài Thực Hành</span></td>
                  <td><code>CRIT-3.1-A</code></td>
                  <td>Thiết kế và xây dựng RESTful Web API an toàn</td>
                  <td><span className="badge badge-cyan">CLO1</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--text-primary)' }}>40.0%</td>
                  <td rowSpan={2} style={{ textAlign: 'center', verticalAlign: 'middle', borderLeft: '1px solid var(--border-subtle)' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td><span className="badge badge-secondary">A3: Đồ Án Cuối Kỳ</span></td>
                  <td><code>CRIT-3.1-B</code></td>
                  <td>Tích hợp cơ sở dữ liệu và triển khai hệ thống hoàn chỉnh</td>
                  <td><span className="badge badge-cyan">CLO2</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--text-primary)' }}>60.0%</td>
                </tr>

                <tr>
                  <td rowSpan={2} style={{ fontWeight: 800, color: 'var(--primary-400)', verticalAlign: 'middle', borderRight: '1px solid var(--border-subtle)' }}>
                    PI 5.1
                    <div style={{ fontSize: '0.72rem', color: 'var(--text-secondary)', fontWeight: 400 }}>Kiểm thử và đánh giá phần mềm</div>
                  </td>
                  <td><span className="badge badge-secondary">A2: Bài Thực Hành</span></td>
                  <td><code>CRIT-5.1-A</code></td>
                  <td>Viết Unit Test & Tích hợp kiểm thử tự động CI/CD</td>
                  <td><span className="badge badge-cyan">CLO3</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--text-primary)' }}>50.0%</td>
                  <td rowSpan={2} style={{ textAlign: 'center', verticalAlign: 'middle', borderLeft: '1px solid var(--border-subtle)' }}>
                    <span className="badge badge-success">Tổng: 100.0% ✓</span>
                  </td>
                </tr>
                <tr>
                  <td><span className="badge badge-secondary">A3: Đồ Án Cuối Kỳ</span></td>
                  <td><code>CRIT-5.1-B</code></td>
                  <td>Báo cáo kiểm thử bảo mật và hiệu năng tải</td>
                  <td><span className="badge badge-cyan">CLO3</span></td>
                  <td>10.0</td>
                  <td style={{ textAlign: 'center', fontWeight: 700, color: 'var(--text-primary)' }}>50.0%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB: BẢNG 8.3.1 - TRUY VẾT TOÀN DIỆN (FR-PRT-17) */}
      {activeTab === 'table831' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <FileSpreadsheet size={20} className="text-primary-400" />
                Bảng 8.3.1: Ma Trận Truy Vết Toàn Diện CLO - PI - Bài Đánh Giá - Tiêu Chí - Minh Chứng (FR-PRT-17)
              </h3>
              <p className="glass-card-subtitle">
                Phân biệt rõ: Đo trực tiếp (Direct A), Hỗ trợ (Supporting), Chỉ đánh giá CLO môn học
              </p>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Chuẩn Đầu Ra CLO</th>
                  <th>Chỉ Số PI Liên Kết</th>
                  <th>Bài Đánh Giá (Item)</th>
                  <th>Tiêu Chí Rubric</th>
                  <th>Phân Loại Vai Trò</th>
                  <th>Minh Chứng Lưu Trữ</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><span className="badge badge-cyan">CLO1</span></td>
                  <td><strong>PI 3.1</strong></td>
                  <td>A1 (Chuyên cần & Quiz)</td>
                  <td>Câu hỏi trắc nghiệm kiến thức C#</td>
                  <td><span className="badge badge-secondary">Chỉ đánh giá CLO</span></td>
                  <td>Log hệ thống LMS Canvas</td>
                </tr>
                <tr>
                  <td><span className="badge badge-cyan">CLO2</span></td>
                  <td><strong>PI 3.1</strong></td>
                  <td>A2 (Thực hành Web API)</td>
                  <td>CRIT-3.1-A (RESTful API)</td>
                  <td><span className="badge badge-danger">Đo Trực Tiếp (A)</span></td>
                  <td>Source code GitHub & Phiếu chấm</td>
                </tr>
                <tr>
                  <td><span className="badge badge-cyan">CLO3</span></td>
                  <td><strong>PI 5.1</strong></td>
                  <td>A3 (Đồ án cuối kỳ)</td>
                  <td>CRIT-5.1-A (Unit Testing)</td>
                  <td><span className="badge badge-danger">Đo Trực Tiếp (A)</span></td>
                  <td>Báo cáo kiểm thử & Video demo</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB: EXAM BLUEPRINT (FR-PRT-03, 05) */}
      {activeTab === 'blueprint' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Cấu Trúc Đề Thi & Ngân Hàng Câu Hỏi (Exam Blueprint)</h3>
              <p className="glass-card-subtitle">
                Phân bổ mức Bloom và tỷ trọng điểm số theo chuẩn khảo thí
              </p>
            </div>
            <button className="btn btn-sm btn-primary">+ Tạo Cấu Trúc Đề Thi</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: '1rem' }}>
            {[
              { section: 'Phần 1: Kiến thức Cơ bản', questions: '10 Câu trắc nghiệm', score: '3.0 điểm', bloom: 'REMEMBER / UNDERSTAND' },
              { section: 'Phần 2: Viết Code Xử lý', questions: '2 Bài tập lập trình', score: '4.0 điểm', bloom: 'APPLY (Mức 3)' },
              { section: 'Phần 3: Tối ưu & Thiết kế', questions: '1 Bài toán kiến trúc', score: '3.0 điểm', bloom: 'ANALYZE (Mức 4)' },
            ].map((s, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <strong style={{ color: 'var(--text-primary)', fontSize: '0.95rem' }}>{s.section}</strong>
                <div style={{ marginTop: '0.5rem', fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
                  <div>• Cấu trúc: {s.questions}</div>
                  <div>• Điểm số: <strong style={{ color: 'var(--primary-400)' }}>{s.score}</strong></div>
                  <div>• Mức Bloom: <span className="badge badge-bloom badge-cyan">{s.bloom}</span></div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB: DOCUMENT VAULT (FR-PRT-07, 11, 12) */}
      {activeTab === 'vault' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <FolderArchive size={20} className="text-primary-400" />
                Kho Tài Liệu Học Thuật & Gói Portfolio Học Phần (Document Vault)
              </h3>
              <p className="glass-card-subtitle">
                Quét mã độc tự động, bảo vệ toàn vẹn bằng mã băm SHA-256 và Watermark bảo mật
              </p>
            </div>
            <button className="btn btn-sm btn-primary">+ Tải Lên Tài Liệu Mới</button>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Tên File & Tài Liệu</th>
                  <th>Loại Minh Chứng</th>
                  <th>Dung Lượng</th>
                  <th>Mã Băm SHA-256 (Toàn Vẹn)</th>
                  <th>Quét An Toàn</th>
                  <th>Thao Tác</th>
                </tr>
              </thead>
              <tbody>
                {[
                  { name: 'IT4101_DeCuong_BM13_2023.pdf', type: 'Đề cương chi tiết', size: '2.4 MB', hash: 'e8f7a9...3b1c', safe: true },
                  { name: 'IT4101_Rubric_ChamDiem_A2.xlsx', type: 'Phiếu chấm Rubric', size: '1.1 MB', hash: 'c4d5e6...7f8a', safe: true },
                  { name: 'IT4101_DeThi_DapAn_CuoiKy.pdf', type: 'Đề thi & Đáp án', size: '3.8 MB', hash: 'a1b2c3...9d0e', safe: true },
                ].map((doc, i) => (
                  <tr key={i}>
                    <td style={{ fontWeight: 600 }}>{doc.name}</td>
                    <td><span className="badge badge-primary">{doc.type}</span></td>
                    <td>{doc.size}</td>
                    <td><code style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{doc.hash}</code></td>
                    <td><span className="badge badge-success"><ShieldCheck size={12} /> Sạch</span></td>
                    <td>
                      <button className="btn btn-sm btn-secondary">Tải Xuống</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};
