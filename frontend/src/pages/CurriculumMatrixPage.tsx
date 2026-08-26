import React, { useState } from 'react';
import {
  Network,
  GitBranch,
  Layers,
  CheckCircle,
  AlertCircle,
  Sparkles,
  Download,
  Share2,
} from 'lucide-react';

interface MatrixCell {
  courseCode: string;
  courseName: string;
  semester: number;
  plos: Record<string, 'I' | 'R' | 'M' | 'A' | 'RA' | 'MA' | '-'>;
}

export const CurriculumMatrixPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'matrix' | 'coverage' | 'prerequisites' | 'bloom'>('matrix');

  // Matrix Sample Dataset
  const matrixData: MatrixCell[] = [
    { courseCode: 'IT1101', courseName: 'Nhập môn Lập trình', semester: 1, plos: { PLO1: 'I', PLO2: 'I', PLO3: 'I', PLO4: '-', PLO5: '-', PLO6: 'I' } },
    { courseCode: 'IT2102', courseName: 'Cấu trúc Dữ liệu & Giải thuật', semester: 2, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: '-', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3101', courseName: 'Cơ sở Dữ liệu & SQL', semester: 3, plos: { PLO1: 'R', PLO2: 'R', PLO3: 'R', PLO4: 'I', PLO5: 'I', PLO6: '-' } },
    { courseCode: 'IT3202', courseName: 'Phân tích & Thiết kế HTTT', semester: 4, plos: { PLO1: 'R', PLO2: 'M', PLO3: 'R', PLO4: 'R', PLO5: 'R', PLO6: 'R' } },
    { courseCode: 'IT4101', courseName: 'Lập trình .NET Nâng cao', semester: 5, plos: { PLO1: 'M', PLO2: 'M', PLO3: 'A', PLO4: 'R', PLO5: 'RA', PLO6: 'R' } },
    { courseCode: 'IT4205', courseName: 'Kiểm thử Phần mềm & QA', semester: 6, plos: { PLO1: 'M', PLO2: 'R', PLO3: 'R', PLO4: 'R', PLO5: 'MA', PLO6: 'R' } },
    { courseCode: 'IT4999', courseName: 'Khóa luận Tốt nghiệp', semester: 8, plos: { PLO1: 'A', PLO2: 'A', PLO3: 'MA', PLO4: 'MA', PLO5: 'MA', PLO6: 'MA' } },
  ];

  return (
    <div className="animate-fade-in">
      {/* Header Banner */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <div>
          <h2 style={{ fontSize: '1.75rem', fontWeight: 800, color: 'var(--text-primary)', letterSpacing: '-0.02em' }}>
            Ma Trận Chuẩn Đầu Ra & Cấu Trúc CTĐT (Mục 8.2)
          </h2>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', marginTop: '0.25rem' }}>
            Quản lý ma trận 2 chiều (I/R/M/A), phân tích độ phủ StudentPath và sơ đồ tiên quyết theo chuẩn ABET / AUN-QA.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn btn-secondary">
            <Download size={16} />
            <span>Xuất Ma Trận Excel</span>
          </button>
          <button className="btn btn-primary">
            <Sparkles size={16} />
            <span>AI Kiểm Tra Mâu Thuẫn Bloom</span>
          </button>
        </div>
      </div>

      {/* Navigation Tabs */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem', borderBottom: '1px solid var(--border-medium)', paddingBottom: '0.5rem' }}>
        {[
          { key: 'matrix', label: 'Ma Trận Học Phần - PLO (I/R/M/A)', icon: Network },
          { key: 'coverage', label: 'Phân Tích Độ Phủ StudentPath', icon: CheckCircle },
          { key: 'prerequisites', label: 'Sơ Đồ Môn Tiên Quyết (DAG)', icon: GitBranch },
          { key: 'bloom', label: 'Lộ Trình Năng Lực Bloom', icon: Layers },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key as any)}
            className={`btn ${activeTab === tab.key ? 'btn-primary' : 'btn-secondary'}`}
            style={{ fontSize: '0.8125rem' }}
          >
            <tab.icon size={16} />
            <span>{tab.label}</span>
          </button>
        ))}
      </div>

      {/* TAB 1: 2D MATRIX (I/R/M/A) */}
      {activeTab === 'matrix' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">Ma Trận Đóng Góp CĐR Của Các Học Phần (CTĐT K17 - 145 Tín chỉ)</h3>
              <p className="glass-card-subtitle">
                Quy ước: <strong>I</strong> = Introduce (Giới thiệu), <strong>R</strong> = Reinforce (Củng cố), <strong>M</strong> = Master (Thuần thục), <strong>A</strong> = Direct Assessment (Đo trực tiếp)
              </p>
            </div>
            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <span className="badge badge-primary">I: Giới thiệu</span>
              <span className="badge badge-cyan">R: Củng cố</span>
              <span className="badge badge-success">M: Thuần thục</span>
              <span className="badge badge-danger">A: Đo trực tiếp</span>
            </div>
          </div>

          <div className="table-container">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Mã Môn</th>
                  <th>Tên Học Phần</th>
                  <th>Học Kỳ</th>
                  <th style={{ textAlign: 'center' }}>PLO1 (Kiến thức)</th>
                  <th style={{ textAlign: 'center' }}>PLO2 (Thiết kế)</th>
                  <th style={{ textAlign: 'center' }}>PLO3 (Lập trình)</th>
                  <th style={{ textAlign: 'center' }}>PLO4 (Kỹ năng mềm)</th>
                  <th style={{ textAlign: 'center' }}>PLO5 (Kiểm thử)</th>
                  <th style={{ textAlign: 'center' }}>PLO6 (Đạo đức)</th>
                </tr>
              </thead>
              <tbody>
                {matrixData.map((row) => (
                  <tr key={row.courseCode}>
                    <td><strong className="badge badge-secondary">{row.courseCode}</strong></td>
                    <td style={{ fontWeight: 600 }}>{row.courseName}</td>
                    <td>HK {row.semester}</td>
                    {['PLO1', 'PLO2', 'PLO3', 'PLO4', 'PLO5', 'PLO6'].map((plo) => {
                      const val = row.plos[plo] || '-';
                      let badgeClass = 'badge-secondary';
                      if (val.includes('A')) badgeClass = 'badge-danger';
                      else if (val === 'M') badgeClass = 'badge-success';
                      else if (val === 'R') badgeClass = 'badge-cyan';
                      else if (val === 'I') badgeClass = 'badge-primary';

                      return (
                        <td key={plo} style={{ textAlign: 'center' }}>
                          {val !== '-' ? (
                            <span className={`badge ${badgeClass}`} style={{ minWidth: '38px', justifyContent: 'center', fontWeight: 800 }}>
                              {val}
                            </span>
                          ) : (
                            <span style={{ color: 'var(--text-muted)' }}>-</span>
                          )}
                        </td>
                      );
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* TAB 2: STUDENT PATH COVERAGE (FR-CTD-10) */}
      {activeTab === 'coverage' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <CheckCircle size={20} className="text-emerald-400" />
                Phân Tích Độ Phủ CĐR Theo Từng StudentPath (FR-CTD-10)
              </h3>
              <p className="glass-card-subtitle">
                Đảm bảo mọi sinh viên theo học bất kỳ lộ trình nào (Chuyên ngành, Tự chọn) đều được phủ 100% CĐR
              </p>
            </div>
            <span className="badge badge-success">Độ phủ: 100.0%</span>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1.25rem' }}>
            {[
              { path: 'Lộ trình 1: Kỹ Thuật Phần Mềm Web', status: 'HỢP LỆ (100%)', plos: 6, pis: 18, aSources: 'Đầy đủ nguồn A' },
              { path: 'Lộ trình 2: Kỹ Thuật Phần Mềm Di Động', status: 'HỢP LỆ (100%)', plos: 6, pis: 18, aSources: 'Đầy đủ nguồn A' },
              { path: 'Lộ trình 3: Trí Tuệ Nhân Tạo & Dữ Liệu', status: 'HỢP LỆ (100%)', plos: 6, pis: 18, aSources: 'Đầy đủ nguồn A' },
            ].map((p, idx) => (
              <div key={idx} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.75rem' }}>
                  <strong style={{ color: 'var(--text-primary)' }}>{p.path}</strong>
                  <span className="badge badge-success">{p.status}</span>
                </div>
                <div style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)', display: 'flex', flexDirection: 'column', gap: '0.35rem' }}>
                  <div>• Độ phủ: <strong>{p.plos}/6 PLO</strong> ({p.pis} chỉ số PI)</div>
                  <div>• Nguồn đo trực tiếp: <strong style={{ color: 'var(--emerald-400)' }}>{p.aSources}</strong></div>
                  <div>• Kiểm tra mức M: Đạt yêu cầu thuần thục trước khi tốt nghiệp</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* TAB 3: PREREQUISITES GRAPH (FR-CTD-21) */}
      {activeTab === 'prerequisites' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <GitBranch size={20} className="text-primary-400" />
                Sơ Đồ Đồ Thị Môn Tiên Quyết (Prerequisite DAG - FR-CTD-21)
              </h3>
              <p className="glass-card-subtitle">
                Đường dẫn tới Khóa luận Tốt nghiệp và các điều kiện ràng buộc học phần
              </p>
            </div>
          </div>

          <div style={{ padding: '2rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)', display: 'flex', flexDirection: 'column', gap: '1.5rem', alignItems: 'center' }}>
            <div style={{ display: 'flex', gap: '1.5rem', alignItems: 'center', flexWrap: 'wrap', justifyContent: 'center' }}>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(99, 102, 241, 0.2)', border: '1px solid var(--primary-500)', borderRadius: 'var(--radius-md)', textAlign: 'center' }}>
                <strong style={{ color: '#fff' }}>IT1101</strong>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Nhập môn Lập trình (HK1)</div>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(6, 182, 212, 0.2)', border: '1px solid var(--cyan-500)', borderRadius: 'var(--radius-md)', textAlign: 'center' }}>
                <strong style={{ color: '#fff' }}>IT2102</strong>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Cấu trúc Dữ liệu (HK2)</div>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(16, 185, 129, 0.2)', border: '1px solid var(--emerald-500)', borderRadius: 'var(--radius-md)', textAlign: 'center' }}>
                <strong style={{ color: '#fff' }}>IT4101</strong>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Lập trình .NET (HK5)</div>
              </div>
              <span style={{ color: 'var(--primary-400)', fontWeight: 800 }}>➔</span>
              <div style={{ padding: '0.75rem 1.25rem', backgroundColor: 'rgba(244, 63, 94, 0.2)', border: '1px solid var(--rose-500)', borderRadius: 'var(--radius-md)', textAlign: 'center' }}>
                <strong style={{ color: '#fff' }}>IT4999</strong>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Khóa luận Tốt nghiệp (HK8)</div>
              </div>
            </div>

            <div style={{ fontSize: '0.8125rem', color: 'var(--text-secondary)' }}>
              Đường găng (Critical Path): <strong>IT1101 ➔ IT2102 ➔ IT4101 ➔ IT4999</strong> (Tổng 4 học kỳ tích lũy).
            </div>
          </div>
        </div>
      )}

      {/* TAB 4: BLOOM PROGRESSION (FR-CTD-11) */}
      {activeTab === 'bloom' && (
        <div className="glass-card">
          <div className="glass-card-header">
            <div>
              <h3 className="glass-card-title">
                <Layers size={20} className="text-cyan-400" />
                Lộ Trình Phát Triển Năng Lực Bloom 6 Cấp Độ Theo Học Kỳ
              </h3>
              <p className="glass-card-subtitle">
                Đảm bảo sinh viên phát triển từ mức Nhận biết/Hiểu (Năm 1-2) lên Phân tích/Đánh giá/Sáng tạo (Năm 3-4)
              </p>
            </div>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1rem' }}>
            {[
              { year: 'Năm 1 (HK 1 - 2)', bloom: 'REMEMBER & UNDERSTAND (Mức 1-2)', desc: 'Tiếp thu kiến thức cơ bản, cú pháp ngôn ngữ và toán tin.' },
              { year: 'Năm 2 (HK 3 - 4)', bloom: 'APPLY (Mức 3)', desc: 'Vận dụng kiến thức cơ sở dữ liệu và thuật toán vào bài tập thực hành.' },
              { year: 'Năm 3 (HK 5 - 6)', bloom: 'ANALYZE & EVALUATE (Mức 4-5)', desc: 'Phân tích kiến trúc hệ thống, kiểm thử phần mềm và đánh giá giải pháp.' },
              { year: 'Năm 4 (HK 7 - 8)', bloom: 'CREATE (Mức 6)', desc: 'Sáng tạo và hoàn thiện sản phẩm đồ án, khóa luận tốt nghiệp thực tế.' },
            ].map((b, i) => (
              <div key={i} style={{ padding: '1.25rem', backgroundColor: 'var(--bg-surface-elevated)', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-medium)' }}>
                <h4 style={{ color: 'var(--primary-400)', fontSize: '0.95rem', marginBottom: '0.5rem' }}>{b.year}</h4>
                <span className="badge badge-bloom badge-cyan" style={{ marginBottom: '0.5rem', display: 'inline-block' }}>{b.bloom}</span>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', lineHeight: '1.4' }}>{b.desc}</p>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
